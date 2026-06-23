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



async function getAllClasses() {
    const dropdown = document.getElementById("className");
    try {
        const response = await fetch("https://localhost:7098/api/admin/get/classes", {
            method: "GET",
            headers: {
                "Content-type": "application/json"
            }
        });
        const classes = await response.json();

        classes.forEach(item => {
            const option = document.createElement("option");

            option.value = item.csId;
            option.textContent = `${item.className} - ${item.sectionName}`;

            dropdown.appendChild(option);
        });

    }
    catch (err) {
        console.error(err);
    }
}
getAllClasses();


document.getElementById("className").addEventListener("change", async function () {
    var selected_option = parseInt(document.getElementById("className").value);
    console.log(selected_option);

    document.getElementById("selectedCSId").value =
        selected_option;

    const selectedText =
        this.options[this.selectedIndex].text;

    document.getElementById("selectedClassName").textContent =
        this.value
            ? selectedText
            : "Not Selected";

    var AdmissionPgTableBody = document.getElementById("AdmissionPageFeeTypesTable");
    try {
        const response = await fetch(`https://localhost:7098/api/adm/get/classSectionFeeType/${selected_option}`, {
            method: "GET",
            headers: {
                "Content-type": "application/json"
            }
        });

        const res = await response.json();

        if (res.fmLists.length > 0) {
            document.getElementById("outerDivTable").classList.remove("visually-hidden");

            AdmissionPgTableBody.innerHTML = "";


            res.fmLists.forEach(item => {
                AdmissionPgTableBody.innerHTML += `
                    <tr>
                        <td>
                            <input type="checkbox" data-cftid="${item.classFeeTypeId}"
                            data-ftid="${item.feeTypeId}"
                              class="admPgCheckBoxSelect"
                            />   
                        </td>
                        <td>${item.feeTypeName}</td>
                        <td id="fs-fixedAmt-${item.classFeeTypeId}">${item.amount}</td>
                        <td>
                            <input type="number" id="fs-disc-${item.classFeeTypeId}"
                            data-amount="${item.amount}"
                            value="0"
                            disabled/>   
                        </td>
                        <td>
                             <input type="number" id="fs-finalAmt-${item.classFeeTypeId}" value="${item.amount}"
                             readonly/>   
                        </td>
                    </tr>
                `;
            });

            document.querySelectorAll(".admPgCheckBoxSelect").forEach(cb => {
                cb.addEventListener("change", function () {
                    const cftId = this.dataset.cftid;

                    const discountInput = document.getElementById(
                        `fs-disc-${cftId}`
                    );

                    if (this.checked) {
                        discountInput.disabled = false;
                    }
                    else {
                        discountInput.disabled = true;
                        discountInput.value = 0;

                        const amount = parseFloat(discountInput.dataset.amount);

                        document.getElementById(
                            `fs-finalAmt-${cftId}`
                        ).value = amount;
                    }
                })
             });

            document.querySelectorAll("[id^='fs-disc-']").forEach(input => {

                input.addEventListener("input", function () {

                    const amount = parseFloat(this.dataset.amount);
                    let discount = parseFloat(this.value) || 0;

                    if (discount > amount) {
                        discount = amount;
                        this.value = amount;
                    }

                    document.getElementById(
                        `fs-finalAmt-${this.id.replace("fs-disc-", "")}`
                    ).value = amount - discount;
                });

            });
        }
        else {
            document.getElementById("outerDivTable").classList.add("visually-hidden");
            iziToast.error({
                title: "ERROR!",
                message : "No fee mapping for this class found! Try mapping the fees first or select another class"
            })
            AdmissionPgTableBody.innerHTML = "";
            return;
        }

        
    }
    catch (err) {
        console.error(err);
    }
    finally {
        document.getElementById("className").value = "";
    }

});


async function admitStudentbtnclick() {
    const csid = parseInt(
        document.getElementById("selectedCSId").value
    );
    console.log("This is the csid "+ csid);
    const stdId = document.getElementById("hiddenFieldStudentId").value;
    if (stdId == "") {
        iziToast.error({
            title: "ERROR!",
            message: "Please select the student first"
        })
        return;
    }
    const selectedFees = [];

    document.querySelectorAll(".admPgCheckBoxSelect:checked").forEach(checkbox => {
        const cftid = checkbox.dataset.cftid;
        const ftid = checkbox.dataset.ftid;
        const discount = document.getElementById(`fs-disc-${cftid}`).value;
        const fixedAmt = document.getElementById(`fs-fixedAmt-${cftid}`).textContent;
        const finalAmount = document.getElementById(`fs-finalAmt-${cftid}`).value;
        selectedFees.push({
            ftid: parseInt(ftid),
            fixedAmt: parseInt(fixedAmt),
            classFeeTypeId: parseInt(cftid),
            discount: parseFloat(discount),
            finalAmount: parseFloat(finalAmount)
        });
    });

    if (selectedFees.length == 0) {
        iziToast.error({
            title: "ERROR!",
            message: "Please select atleast one of the give fee types"
        })
        return;
    }


    const bodyObj = {
        csId: csid,
        stdId: stdId,
        SelectedFeesList: selectedFees
    }

    console.log(bodyObj);


    try {
        const response = await fetch("https://localhost:7098/api/adm/student/addmission", {
            method: "POST",
            headers: {
                "Content-type": "application/json"
            },
            body: JSON.stringify(bodyObj)
        });

        const res = await response.json();
        console.log(res);
        if (res.success) {
            iziToast.success({
                title: "SUCCESS",
                message: "Student admitted successfully!"
            });
        }

    }
    catch (err) {
        console.error(err);
        iziToast.error({
            title: "ERROR!",
            message: "Error admitting the student"
        })
    }
    finally {
        document.getElementById("selectedCSId").value = "";
        document.getElementById("className").value = "";
        document.getElementById("outerDivTable").classList.add("visually-hidden");
        document.getElementById("selectedStudentName").textContent="Not Selected";
        document.getElementById("selectedClassName").textContent="Not Selected";
    }
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



    var AdmissionPgTableBody = document.getElementById("AdmissionPageFeeTypesTable");
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

            document.getElementById("btnSubmitStudent").classList.add("visually-hidden");

            document.getElementById("btnFeeUpdateStudent").classList.remove("visually-hidden");

            document.getElementById("selectedClassName").textContent = `${res.feeLst[0].className} - ${res.feeLst[0].sectionName}`;

            document.getElementById("selectedStudentAdmissionId").value = res.feeLst[0].admissionId;

            // Disabling the classlist dropdown when we are updating the fee details for a existing admitted student
            document.getElementById("className").disabled = true;

            AdmissionPgTableBody.innerHTML = "";


            res.feeLst.forEach(item => {
                AdmissionPgTableBody.innerHTML += `
                    <tr>
                        <td>
                            <input type="checkbox" data-cftid="${item.classFeeTypeId}"
                            data-ftid="${item.feeTypeId}"
                              class="admPgCheckBoxSelect"
                              ${item.checked==1 ? "checked" : ""}
                            />   
                        </td>
                        <td>${item.feeTypeName}</td>
                        <td id="fs-fixedAmt-${item.classFeeTypeId}">${item.defaultAmount}</td>
                        <td>
                            <input type="number" id="fs-disc-${item.classFeeTypeId}"
                            data-amount="${item.defaultAmount}"
                            value="${item.discount}"
                            ${item.checked == 0 ? "disabled" : ""}

                            />   
                        </td>
                        <td>
                             <input type="number" id="fs-finalAmt-${item.classFeeTypeId}" value="${item.checked == 1 ? item.finalAmount : item.defaultAmount}"
                             readonly/>   
                        </td>
                    </tr>
                `;
            });

            document.querySelectorAll(".admPgCheckBoxSelect").forEach(cb => {
                cb.addEventListener("change", function () {
                    const cftId = this.dataset.cftid;

                    const discountInput = document.getElementById(
                        `fs-disc-${cftId}`
                    );

                    if (this.checked) {
                        discountInput.disabled = false;
                    }
                    else {
                        discountInput.disabled = true;
                        discountInput.value = 0;

                        const amount = parseFloat(discountInput.dataset.amount);

                        document.getElementById(
                            `fs-finalAmt-${cftId}`
                        ).value = amount;
                    }
                })
            });

            document.querySelectorAll("[id^='fs-disc-']").forEach(input => {

                input.addEventListener("input", function () {

                    const amount = parseFloat(this.dataset.amount);
                    let discount = parseFloat(this.value) || 0;

                    if (discount > amount) {
                        discount = amount;
                        this.value = amount;
                    }

                    document.getElementById(
                        `fs-finalAmt-${this.id.replace("fs-disc-", "")}`
                    ).value = amount - discount;
                });

            });
        }
        else {
            AdmissionPgTableBody.innerHTML = "";
            return;
        }


    }
    catch (err) {
        console.error(err);
    }
    finally {
        document.getElementById("className").value = "";
    }
}


async function updateFeeForAlreadyAdmittedStudentbtnclick() {
    try {
        const selectedFees = [];
        const admissionId = document.getElementById("selectedStudentAdmissionId").value;

        document.querySelectorAll(".admPgCheckBoxSelect:checked").forEach(checkbox => {
            const cftid = checkbox.dataset.cftid;
            const ftid = checkbox.dataset.ftid;
            const discount = document.getElementById(`fs-disc-${cftid}`).value;
            const fixedAmt = document.getElementById(`fs-fixedAmt-${cftid}`).textContent;
            const finalAmount = document.getElementById(`fs-finalAmt-${cftid}`).value;
            selectedFees.push({
                ftid: parseInt(ftid),
                fixedAmt: parseInt(fixedAmt),
                classFeeTypeId: parseInt(cftid),
                discount: parseFloat(discount),
                finalAmount: parseFloat(finalAmount)
            });
        });

        var bodyObj = {
            admissionId: admissionId,
            selectedFees: selectedFees
        }

        console.log(bodyObj);


        const response = await fetch("https://localhost:7098/api/adm/update/feeDetails/student", {
            method: "POST",
            headers: {
                "Content-type": "application/json"
            },
            body: JSON.stringify(bodyObj)
        });

        const res = await response.json();

        if (res.success) {
            iziToast.success({
                title: "SUCCESS",
                message: "Student fee details updated successfully!"
            });
        }
  
    }
    catch (err) {
        console.error(err);
        iziToast.error({
            title: "ERROR!",
            message: "Error updating student fee details"
        })    }
    finally {
        document.getElementById("selectedCSId").value = "";
        document.getElementById("className").value = "";
        document.getElementById("outerDivTable").classList.add("visually-hidden");
        document.getElementById("selectedStudentName").textContent = "Not Selected";
        document.getElementById("selectedClassName").textContent = "Not Selected";
    }
}