async function searchStudentToE() {
    var tableBody = document.getElementById("StudentsSearchList");
    var searchStudentName = document.getElementById("stdntName").value;
    tableBody.innerHTML = "";

    try {
        const api_body = {
            "FDate": null,
            "EDate": null,
            "SearchText": searchStudentName
        };

        const response = await fetch("https://localhost:7098/api/adm/student/search", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(api_body)
        });

        const students = await response.json();


        let index = 1;
        console.log(students);
        students.forEach(student => {
            tableBody.innerHTML += `
            <tr onClick="selectStudent(${student.sId},'${student.sName}')" data-bs-dismiss="modal">
                <td> ${index++}</td>       
                <td> ${student.sName}</td>         
                <td> ${student.studentContact}</td>         
                <td> ${student.sdob.split(" ")[0]}</td>         
            </tr>`;
        });

    }
    catch (err) {
        console.log(err);
    }

    document.getElementById("stdntName").value = "";
}



async function selectStudent(studentId, studentName) {

    // Store Student ID
    document.getElementById("hiddenFieldStudentId").value = studentId;


    // Show Selected Student
    document.getElementById("selectedStudentName").textContent =
        studentName;

    // Close Modal
    const modal =
        bootstrap.Modal.getInstance(
            document.getElementById("studentSearchModal")
        );

    if (modal) {
        modal.hide();
    }


    var FeePgFeeTypesTable = document.getElementById("FeePgFeeTypesTable");
    try {
        const response = await fetch(`https://localhost:7098/api/adm/get/balanceFee/student/${studentId}`, {
            method: "GET",
            headers: {
                "Content-type": "application/json"
            }
        });

        const res = await response.json();

        if (res.data.length > 0) {
            document.getElementById("outerDivTable").classList.remove("visually-hidden");
            document.getElementById("admissionIdSelectedStudent").value = res.data[0].admissionId;

            document.getElementById("selectedClassName").textContent = `${res.data[0].className} - ${res.data[0].sectionName}`;

            FeePgFeeTypesTable.innerHTML = "";

            var index = 1;
            res.data.forEach(item => {
                    FeePgFeeTypesTable.innerHTML += `
                    <tr class="feePgDataRow" data-afid=${item.admissionFeeId}>
                        <td>${index++}</td>

                        <td>${item.feeTypeName}</td>

                        <td id="fs-totalAmt-${item.admissionFeeId}">${item.totalAmt}</td>
                        <td id="fs-totalPaid-${item.admissionFeeId}">${item.totalPaid}</td>
                        
                        <td>
                            <input type="number" id="fs-amt-${item.admissionFeeId}"
                            data-amount="${item.balanceAmt}"
                            value="0"
                            />   
                        </td>

                        <td>
                             <input type="number" id="fs-balanceAmt-${item.admissionFeeId}" value="${item.balanceAmt}"
                             readonly/>   
                        </td>
                    </tr>
                `;
            });

            // placeholder = "Paid"

        document.querySelectorAll("[id^='fs-amt-']").forEach(input => {

            input.addEventListener("input", function () {

                const balAmt = parseFloat(this.dataset.amount);
                let amt = parseFloat(this.value) || 0;

                if (amt > balAmt) {
                    amt = balAmt;
                    this.value = balAmt;
                }

                document.getElementById(
                    `fs-balanceAmt-${this.id.replace("fs-amt-", "")}`
                ).value = balAmt - amt;
            });

        });
    }
        else {
            document.getElementById("outerDivTable").classList.add("visually-hidden");
            iziToast.error({
                title: "ERROR!",
                message: "Student is Not admitted! Please admit the student first"
            })
            FeePgFeeTypesTable.innerHTML = "";
            return;
        }
    }
    catch (err) {
        console.log(err);
    }
    finally {
  
    }
}


async function savePaymentDetailsBtnClick() {
    const admId = document.getElementById("admissionIdSelectedStudent").value;
    const paymentType = document.getElementById("paymentType").value; // shouldnt be empty(i.e. no option selected) : add check
    const paymentDate = document.getElementById("datePayment").value;
    var trxnNo = document.getElementById("trxnPayment").value.trim();
    const remarks = document.getElementById("remarks").value.trim();
    if (paymentType == "cash") trxnNo = "cash";

    if (paymentType == "") {
        iziToast.error({
            title: "ERROR!",
            message: "Payment Type Cannot be empty"
        });
        return;
    }

    if (paymentDate == "") {
        iziToast.error({
            title: "ERROR!",
            message: "Payment Date Cannot be empty"
        });
        return;
    }

    if (trxnNo == "") {
        iziToast.error({
            title: "ERROR!",
            message: "Transaction Number Cannot be empty for Non cash transactions"
        });
        return;
    }

    const feeBalData = [];

    var flag = false;

    document.querySelectorAll(".feePgDataRow").forEach(item => {
        const admFeeId = item.dataset.afid;
        const amtPaid = parseInt(document.getElementById(`fs-amt-${admFeeId}`).value);
        if (amtPaid > 0) flag = true;
        feeBalData.push({
            AdmissionFeeId: admFeeId,
            AmountPaid: amtPaid
        });
    });

    if (!flag) {
        iziToast.error({
            title: "ERROR!",
            message: "Please add the some amount first"
        });
        return;
    }
    const bodyObj = {
        PaymentType: paymentType,
        PaymentDate: paymentDate,
        TransactionId: trxnNo,
        Remarks: remarks,
        AdmissionId : admId,
        PaidFeeList: feeBalData
    }

    console.log(bodyObj)


    try {
        const response = await fetch("https://localhost:7098/api/adm/update/balance/fee/student", {
            method: "POST",
            headers: {
                "Content-type": "application/json"
            },
            body: JSON.stringify(bodyObj)
        });

        const res = await response.json();
       
        if (res.success) {
            iziToast.success({
                title: "OK!",
                message: "Successfully updated balance for the student!"
            });


            // setting the details for the modal
            document.getElementById("ReceiptIdForThePayment").value =
                res.receiptId;

            document.getElementById("modalReceiptNo").innerText =
                res.receiptNumber;

            // showing the payment summary model
            new bootstrap.Modal(
                document.getElementById("paymentSuccessModal")
            ).show();
        }
        else {
            iziToast.error({
                title: "ERROR!",
                message: "Error updating balance for the student!"
            });
        }
    }
    catch (err) {
        console.log(err);
        iziToast.error({
            title: "ERROR!",
            message: "Error updating balance for the student!"
        });
    }
    finally {
        document.getElementById("hiddenFieldStudentId").value = "";
        document.getElementById("selectedStudentName").textContent = "Not Selected";
        document.getElementById("selectedClassName").textContent = "Not selected";
        document.getElementById("admissionIdSelectedStudent").value = "";


        // Clearing Payment Details table
        document.getElementById("paymentType").value = "";
        document.getElementById("datePayment").value = "";
        document.getElementById("trxnPayment").value = "";
        document.getElementById("remarks").value = "";

        // Clearing the table
        document.getElementById("FeePgFeeTypesTable").innerHTML = "";
    }
}


function downloadReceiptFromModal() {
    const receiptId = document.getElementById("ReceiptIdForThePayment").value;

    window.open(`https://localhost:7098/api/adm/download/receipt/${receiptId}`, "_blank");

    document.getElementById("ReceiptIdForThePayment").value="";
}