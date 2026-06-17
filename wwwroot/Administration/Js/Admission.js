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
                              class="admPgCheckBoxSelect"
                            />   
                        </td>
                        <td>${item.feeTypeName}</td>
                        <td id="fs-fixedAmt-${item.classFeeTypeId}">${item.amount}</td>
                        <td>
                            <input type="number" id="fs-disc-${item.classFeeTypeId}"
                            data-amount="${item.amount}"
                            value="0"/>   
                        </td>
                        <td>
                             <input type="number" id="fs-finalAmt-${item.classFeeTypeId}" value="${item.amount}"
                             readonly/>   
                        </td>
                    </tr>
                `;
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
    console.log(document.getElementById("className").value);  
    const csid = document.getElementById("className").value;  
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
        const discount = document.getElementById(`fs-disc-${cftid}`).value;
        const fixedAmt = document.getElementById(`fs-fixedAmt-${cftid}`).textContent;
        const finalAmount = document.getElementById(`fs-finalAmt-${cftid}`).value;
        selectedFees.push({
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


    try {
        const response = await fetch("", {
            method: "POST",
            headers: {
                "Content-type": "application/json"
            },
            body: JSON.stringify({
                StdId: parseInt(stdId),
                CSId: parseInt(csid),
                SelectedFees: selectedFees
            })
        });

        const res = await response.json();

    }
    catch (err) {
        console.error(err);
    }
}


function selectStudent(studentId, studentName) {

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
}