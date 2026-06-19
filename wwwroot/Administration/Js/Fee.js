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
        const response = await fetch(`https://localhost:7098/api/adm/get/fee/student/${studentId}`, {
            method: "GET",
            headers: {
                "Content-type": "application/json"
            }
        });

        const res = await response.json();

        if (res.feeLst.length > 0) {
            document.getElementById("outerDivTable").classList.remove("visually-hidden");

            document.getElementById("selectedClassName").textContent = `${res.feeLst[0].className} - ${res.feeLst[0].sectionName}`;

            FeePgFeeTypesTable.innerHTML = "";

            var index = 1;
            res.feeLst.forEach(item => {
                if (item.checked == 1) {
                    FeePgFeeTypesTable.innerHTML += `
                    <tr class="feePgDataRow">
                        <td>${index++}</td>

                        <td>${item.feeTypeName}</td>

                        <td id="fs-fixedAmt-${item.classFeeTypeId}">${item.finalAmount}</td>
                        
                        <td>
                            <input type="number" id="fs-amt-${item.classFeeTypeId}"
                            data-amount="${item.finalAmount}"
                            value="0"
                            />   
                        </td>

                        <td>
                             <input type="number" id="fs-finalAmt-${item.classFeeTypeId}" value="${item.finalAmount}"
                             readonly/>   
                        </td>
                    </tr>
                `;
                }
            });

        document.querySelectorAll("[id^='fs-amt-']").forEach(input => {

            input.addEventListener("input", function () {

                const finalAmt = parseFloat(this.dataset.amount);
                let amt = parseFloat(this.value) || 0;

                if (amt > finalAmt) {
                    amt = finalAmt;
                    this.value = finalAmt;
                }

                document.getElementById(
                    `fs-finalAmt-${this.id.replace("fs-amt-", "")}`
                ).value = finalAmt - amt;
            });

        });
    }
        else {
            document.getElementById("outerDivTable").classList.add("visually-hidden");
            iziToast.error({
                title: "ERROR!",
                message: "No fee mapping for this class found! Try mapping the fees first or select another class"
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