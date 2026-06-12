function switchTab(tabId) {
    var tab = document.getElementById(tabId);
    var bsTab = new bootstrap.Tab(tab);
    bsTab.show();
}



function getAge(dateString) {
    const today = new Date();
    const birthDate = new Date(dateString);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    // If current month is before birth month, OR it's the birth month 
    // but the current day is before the birth day, subtract one year.
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }

    return age;
}




async function SubmitStudentDetails() {

    if (validations()) {
        var sName = document.getElementById("sname").value;
        // var sAge = document.getElementById("sage").value;
        var sDOB = document.getElementById("sdob").value;
        // var sContact = document.getElementById("scontact").value;
        var sGender = document.querySelector('input[name="sgender"]:checked');
        var sBloodGrp = document.getElementById("sbloodGrp").value;
        var sImg = document.getElementById("sPhotoUpload").files[0];
        var sFatherName = document.getElementById("fname").value;
        var sMotherName = document.getElementById("mname").value;
        var sGuardianContact = document.getElementById("gcontact1").value;
        var sGuardianAltContact = document.getElementById("gcontact2").value;
        var sAddress = document.getElementById("address").value;
        var sAddressCity = document.getElementById("city").value;
        var sAddressState = document.getElementById("state").value;
        var sAddressPincode = document.getElementById("pincode").value;
        var s10thPctg = document.getElementById("10thPerctg").value;
        var s12thPctg = document.getElementById("12thPerctg").value;
        var sJeeRank = document.getElementById("jeeRank").value;
        var sJuniorClgName = document.getElementById("schoolName").value;
        var sGapYears = document.getElementById("gapYrs").value;
        var sGapJustification = document.getElementById("gapYrJustification").value;


        const formData = new FormData();
        formData.append("SName", sName);

        // If hiddenfield value is not empty, then its a edit operation, so will pass the studentId with the request
        if (document.getElementById("HiddenStudentId").value !=
            "") {
            formData.append("SId", document.getElementById("HiddenStudentId").value);
        }

        // Getting the age from the DOB
        var sAge = getAge(sDOB);


        // Getting the list of contacts
        const contacts = [];
        document.querySelectorAll(".contact-number").forEach(contact => {
            if (contact.value.trim != "") {
                contacts.push({
                    contactNo: contact.value.trim()
                });
            }
        });

        console.log("Contacts of the student:", contacts);

        formData.append("SAge", sAge);
        formData.append("SDOB", sDOB);
        // formData.append("SContact", sContact);
        contacts.forEach((contact, index) => {

            formData.append(
                `contactNos[${index}].ContactNo`,
                contact.contactNo
            );

        });

        formData.append("SGender", sGender ? sGender.value : "");

        formData.append("SBloodGrp", sBloodGrp);

        formData.append("SImg", sImg);

        formData.append("SFatherName", sFatherName);
        formData.append("SMotherName", sMotherName);

        formData.append("SGuardianContact", sGuardianContact);
        formData.append("SGuardianAltContact", sGuardianAltContact);

        formData.append("SAddress", sAddress);
        formData.append("SAddressCity", sAddressCity);
        formData.append("SAddressState", sAddressState);
        formData.append("SAddressPincode", sAddressPincode);

        formData.append("S10thPctg", s10thPctg);
        formData.append("S12thPctg", s12thPctg);

        formData.append("SJeeRank", sJeeRank);

        formData.append("SJuniorClgName", sJuniorClgName);

        formData.append("SGapYears", sGapYears);
        formData.append("SGapJustification", sGapJustification);
        formData.append("ExistingImage", document.getElementById("existingImage").value);



        try {
            const response = await fetch("https://localhost:7098/api/adm/addStudent", {
                method: "POST",
                body: formData,
            });
            if (response.ok) {
                if (document.getElementById("HiddenStudentId").value != "") {
                    iziToast.success({
                        title: 'OK',
                        message: 'Successfully updated the student!',
                    });
                }
                else {
                    iziToast.success({
                        title: 'OK',
                        message: 'Successfully added student!',
                    });
                }
                cleanFields();
                // Resetting the hidden field
                if (document.getElementById("HiddenStudentId").value != "") {
                    showSection("student-list-section");
                    document.getElementById("HiddenStudentId").value = "";
                }

                document.getElementById("studentImagePreview").src = "";

                document.getElementById("studentImagePreviewContainer").classList.add("d-none");

                // document.getElementById("existingImage").value = "";
            }
            else {
                const error = await response.text();
                console.log(error);
                // alert("Failed to add student")
                if (document.getElementById("HiddenStudentId").value != "") {
                    iziToast.error({
                        title: 'Error',
                        message: 'Failed to update the student',
                        position: 'center'
                    });
                }
                else {
                    iziToast.error({
                        title: 'Error',
                        message: 'Failed to add the student',
                        position: 'center'
                    });
                }

            }
        }
        catch (err) {
            console.log(err);
            alert("Something went wrong");
        }
    }

}


async function showSection(id) {
    cleanFields();
    document.getElementById("studentImagePreview").src = "";

    document.getElementById("studentImagePreviewContainer").classList.add("d-none");


    const sections = document.querySelectorAll(".dashboard-section");
    sections.forEach(section => {
        section.classList.add("visually-hidden");
    });

    document.getElementById(id).classList.remove("visually-hidden");

    if (id === "student-list-section") {
        await loadStudents();
    }

    if (id === "class-section") {
        await loadClasses();
    }
    if (id === "enroll-student-section") {
        await classDropdown();
    }

}


async function handleEdit(id) {
    try {
        const response = await fetch(`https://localhost:7098/api/adm/student/${id}`);

        const resp_contacts = await fetch(`https://localhost:7098/api/adm/student/contacts/${id}`)

        const student = await response.json();
        const student_contacts = await resp_contacts.json();
        console.log(student_contacts);

        document.getElementById("HiddenStudentId").value = student.sId;
        document.getElementById("existingImage").value =
            student.sImg;

        document.getElementById("sname").value = student.sName;
        // document.getElementById("sage").value = "";
        document.getElementById("sdob").value =
            student.sdob.split("T")[0];

        document.querySelector(".contact-number").value = student_contacts.contacts[0];

        var idx = 1;
        student_contacts.contacts.forEach(contact => {
            if (idx!=1){
                const container = document.getElementById("contactContainer");

                const row = document.createElement("div");

                row.className = "mb-2 contact-row d-flex gap-2";

                row.innerHTML = `
                <input type="text"
                       class="form-control contact-number"
                       placeholder="Enter Contact Number"
                       maxlength="10" value="${contact}">

                <button type="button"
                        class="btn btn-danger"
                        onclick="removeContact(this)">
                    -
                </button>
            `;

                container.appendChild(row);
            }
            idx++;
        });

        let gender =
            document.querySelector(
                `input[name="sgender"][value="${student.sGender}"]`
            );

        if (gender) {
            gender.checked = true;
        }
        document.getElementById("sbloodGrp").value = student.sBloodGrp;
        if (student.sImg) {
            let imgPrev = document.getElementById("studentImagePreview");
            imgPrev.src = `https://localhost:7098/studentImages/${student.sImg}`;

            document.getElementById("studentImagePreviewContainer").classList.remove("d-none");
        }
        document.getElementById("fname").value = student.sFatherName;
        document.getElementById("mname").value = student.sMotherName;
        document.getElementById("gcontact1").value = student.sGuardianContact;
        document.getElementById("gcontact2").value = student.sGuardianAltContact;
        document.getElementById("address").value = student.sAddress;
        document.getElementById("city").value = student.sAddressCity;
        document.getElementById("state").value = student.sAddressState;
        document.getElementById("pincode").value = student.sAddressPincode;
        document.getElementById("10thPerctg").value = student.s10thPctg;
        document.getElementById("12thPerctg").value = student.s12thPctg;
        document.getElementById("jeeRank").value = student.sJeeRank;
        document.getElementById("schoolName").value = student.sJuniorClgName;
        document.getElementById("gapYrs").value = student.sGapYears;
        document.getElementById("gapYrJustification").value = student.sGapJustification;




    }
    catch (err) {
        console.log(err);
    }
}


async function loadStudents() {
    try {
        const tbody =
            document.getElementById("student-table-body");

        // Clearing old rows
        tbody.innerHTML = "";


        const response = await fetch("https://localhost:7290/api/adm/students");
        const students = await response.json();

        const tableBody = document.getElementById("student-table-body");

        let index = 1;
        students.forEach(student => {
            tableBody.innerHTML += `
            <tr>
                <td> ${index++}</td>       
                <td> ${student.sName}</td>         
                <td> ${student.sFatherName}</td>       
                <td> ${student.sMotherName}</td>       
                <td style="width: 250px;"> ${student.sAddress}</td>       
                <td> <button class="actionCst-btn" onClick="handleEdit(${student.sId})">Edit</button>
                    <button class="actionCst-btn" onClick="handleEdit(${student.sId})">Delete</button>
                </td>       
            </tr>`;
        });
    }
    catch (err) {
        console.log(err);
    }

}


function cleanFields() {
    document.getElementById("sname").value = "";
    // document.getElementById("sage").value = "";
    document.getElementById("sdob").value = "";
    let gender =
        document.querySelector(
            'input[name="sgender"]:checked'
        );

    if (gender) {
        gender.checked = false;
    }
    document.getElementById("sbloodGrp").value = "";
    document.getElementById("sPhotoUpload").value = "";
    document.getElementById("fname").value = "";
    document.getElementById("mname").value = "";
    document.getElementById("gcontact1").value = "";
    document.getElementById("gcontact2").value = "";
    document.getElementById("address").value = "";
    document.getElementById("city").value = "";
    document.getElementById("state").value = "";
    document.getElementById("pincode").value = "";
    document.getElementById("10thPerctg").value = "";
    document.getElementById("12thPerctg").value = "";
    document.getElementById("jeeRank").value = "";
    document.getElementById("schoolName").value = "";
    document.getElementById("gapYrs").value = "";
    document.getElementById("gapYrJustification").value = "";

    const container = document.getElementById("contactContainer");

    container.innerHTML = `
        <div class="mb-2 contact-row d-flex gap-1">
            <input type="text"
                   class="form-control contact-number"
                   placeholder="Enter Contact Number"
                   maxlength="10">

            <button type="button"
                    class="btn btn-success"
                    onclick="addContact()">
                +
            </button>
        </div>
    `;

    switchTab("personal-info-tab");
}

function addContact() {

    const container = document.getElementById("contactContainer");

    const row = document.createElement("div");

    row.className = "mb-2 contact-row d-flex gap-2";

    row.innerHTML = `
        <input type="text"
               class="form-control contact-number"
               placeholder="Enter Contact Number"
               maxlength="10">

        <button type="button"
                class="btn btn-danger"
                onclick="removeContact(this)">
            -
        </button>
    `;

    container.appendChild(row);
}

function removeContact(btn) {
    btn.parentElement.remove();
}

function validatePhoneNumber(input) {
    const regex = /^\d{10}$/; // Matches exactly 10 digits
    return regex.test(input);
}

function validateIndianPinCode(pincode) {
    // Matches exactly 6 digits, avoiding numbers starting with 0
    const regex = /^[1-9]\d{5}$/;
    return regex.test(pincode.trim());
}
function validateNumber(number) {
    // Matches exactly 6 digits, avoiding numbers starting with 0
    const regex = /^-?\d+$/;;
    return regex.test(number.trim());
}

function validations() {

    var sName = document.getElementById("sname").value;
    // var sAge = document.getElementById("sage").value;
    var sDOB = document.getElementById("sdob").value;
    // var sContact = document.getElementById("scontact").value;
    var sGender = document.querySelector('input[name="sgender"]:checked');
    var sBloodGrp = document.getElementById("sbloodGrp").value;
    var sImg = document.getElementById("sPhotoUpload").files[0];
    var sFatherName = document.getElementById("fname").value;
    var sMotherName = document.getElementById("mname").value;
    var sGuardianContact = document.getElementById("gcontact1").value;
    var sGuardianAltContact = document.getElementById("gcontact2").value;
    var sAddress = document.getElementById("address").value;
    var sAddressCity = document.getElementById("city").value;
    var sAddressState = document.getElementById("state").value;
    var sAddressPincode = document.getElementById("pincode").value;
    var s10thPctg = document.getElementById("10thPerctg").value;
    var s12thPctg = document.getElementById("12thPerctg").value;
    var sJeeRank = document.getElementById("jeeRank").value;
    var sJuniorClgName = document.getElementById("schoolName").value;
    var sGapYears = document.getElementById("gapYrs").value;
    var sGapJustification = document.getElementById("gapYrJustification").value;


    if (sName == "") {
        iziToast.error({
            title: 'Error',
            message: 'Please Enter the name of the student!',
            position: 'topRight'
        });
        return false;
    }

    if (sDOB == "") {
        iziToast.error({
            title: 'Error',
            message: 'Please Enter the DOB of the student!',
            position: 'topRight'
        });
        return false;
    }

    // if (sContact == "") {
    //     iziToast.error({
    //         title: 'Error',
    //         message: 'Please Enter the contact number of the student!',
    //         position: 'topRight'
    //     });
    //     return false;
    // }

    // if (!validatePhoneNumber(sContact)) {
    //     iziToast.error({
    //         title: 'Error',
    //         message: 'Please Enter the valid phone number!',
    //         position: 'topRight'
    //     });
    //     return false;
    // }

    let gender =
        document.querySelector(
            'input[name="sgender"]:checked'
        );

    if (!gender) {
        iziToast.error({
            title: 'Error',
            message: 'Please Enter the Gender of the student!',
            position: 'topRight'
        });
        return false;
    }

    if (sBloodGrp == "") {
        iziToast.error({
            title: 'Error',
            message: 'Please Enter the blood group of the student!',
            position: 'topRight'
        });
        return false;
    }

    // This ' document.getElementById("HiddenStudentId").value=="") ' makes the image upload optional when the edit button is clicked
    if (document.getElementById("sPhotoUpload").value == "" && document.getElementById("HiddenStudentId").value == "") {
        iziToast.error({
            title: 'Error',
            message: 'Please Enter the image of the student!',
            position: 'topRight'
        });
        return false;
    }

    if (sFatherName == "") {
        iziToast.error({
            title: 'Error',
            message: "Please Enter the Father's name of the student!",
            position: 'topRight'
        });
        return false;
    }

    if (sMotherName == "") {
        iziToast.error({
            title: 'Error',
            message: "Please Enter the Mother's name of the student!",
            position: 'topRight'
        });
        return false;
    }

    if (sGuardianContact == "") {
        iziToast.error({
            title: 'Error',
            message: "Please Enter the Guardian's contact number!",
            position: 'topRight'
        });
        return false;
    }

    if (!validatePhoneNumber(sGuardianContact)) {
        iziToast.error({
            title: 'Error',
            message: "Please Enter a valid Guardian's contact number!",
            position: 'topRight'
        });
        return false;
    }

    if (sGuardianAltContact == "") {
        iziToast.error({
            title: 'Error',
            message: "Please Enter the Guardian's alternate contact number!",
            position: 'topRight'
        });
        return false;
    }

    if (!validatePhoneNumber(sGuardianAltContact)) {
        iziToast.error({
            title: 'Error',
            message: "Please Enter a valid Guardian's contact number!",
            position: 'topRight'
        });
        return false;
    }

    if (sAddress == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter student's address, Student address cannot be empty!",
            position: 'topRight'
        });

        return false;
    }
    if (sAddressCity == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter city!",
            position: 'topRight'
        });
        return false;
    }
    if (sAddressState == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter the state!",
            position: 'topRight'
        });
        return false;
    }

    if (sAddressPincode == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter the pincode!",
            position: 'topRight'
        });

        return false;
    }

    if (!validateIndianPinCode(sAddressPincode)) {
        iziToast.error({
            title: 'Error',
            message: "Please enter a valid pincode!",
            position: 'topRight'
        });
        return false;
    }

    if (s10thPctg == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter 10th percentage!",
            position: 'topRight'

        });
        return false;
    }

    if (s10thPctg < 0 || s10thPctg > 100) {
        iziToast.error({
            title: 'Error',
            message: "Please enter a valid percentage value(10th Percentage)!",
            position: 'topRight'
        });
        return false;
    }
    if (s12thPctg == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter 12th percentage!",
            position: 'topRight'

        });
        return false;
    }

    if (s12thPctg < 0 || s12thPctg > 100) {
        iziToast.error({
            title: 'Error',
            message: "Please enter a valid percentage value(12th Percentage)!",
            position: 'topRight'
        });

        return false;
    }



    if (sJeeRank == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter JEE rank!",
            position: 'topRight'

        });
        return false;
    }
    if (!validateNumber(sJeeRank)) {
        iziToast.error({
            title: 'Error',
            message: "Please enter valid JEE rank!",
            position: 'topRight'

        });

        return false;
    }


    if (sJuniorClgName == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter Junior clg name!",
            position: 'topRight'

        });
        return false;
    }

    if (sGapYears == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter gap years (If not present, enter 0)!",
            position: 'topRight'

        });
        return false;
    }

    if (sGapJustification == "") {
        iziToast.error({
            title: 'Error',
            message: "Please enter gap justification, if not there enter NA!",
            position: 'topRight'

        });
        return false;
    }


    return true;
}



async function searchStudent() {
    var fromDate = document.getElementById("fromDate").value;
    var toDate = document.getElementById("toDate").value;
    var searchText = document.getElementById("searchBox").value;

    if (searchText == null) return;

    try {
        const tbody =
            document.getElementById("student-table-body");

        // Clearing old rows
        tbody.innerHTML = "";

        const api_body = {
            "FDate": fromDate || null,
            "EDate": toDate || null,
            "SearchText": searchText
        };

        const response = await fetch("https://localhost:7290/api/teacher/student/search", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(api_body)
        });

        const students = await response.json();

        const tableBody = document.getElementById("student-table-body");

        let index = 1;
        console.log(students);
        students.forEach(student => {
            tableBody.innerHTML += `
            <tr>
                <td> ${index++}</td>       
                <td> ${student.sName}</td>       
                <td style="width: 150px;"> ${student.sContact}</td>             
                <td> ${student.sFatherName}</td>       
                <td> ${student.sMotherName}</td>       
                <td style="width: 250px;"> ${student.sAddress}</td>       
                <td> <button class="actionCst-btn" onClick="handleEdit(${student.sId})">Edit</button>
                    <button class="actionCst-btn" onClick="handleEdit(${student.sId})">Delete</button>
                </td>       
            </tr>`;
        });


        // Clearing fields
        document.getElementById("fromDate").value = "";
        document.getElementById("toDate").value = "";
        document.getElementById("searchBox").value = "";
    }
    catch (err) {
        console.log(err);
    }

}

async function addClass() {
    let cName = document.getElementById("className").value;
    let cCapacity = document.getElementById("classCapacity").value;
    try {
        const response = await fetch("https://localhost:7290/api/teacher/addClass", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                CName: cName,
                CCapacity: cCapacity
            })
        });

        const res = await response.json();
        if (res.success == true) {
            iziToast.success({
                title: 'Success',
                message: "Class added successfully!",
                position: 'topRight'

            });
        }
        else {
            iziToast.error({
                title: 'Error',
                message: "Failed to add the Class",
                position: 'topRight'

            });
        }


        document.getElementById("className").value = "";
        document.getElementById("classCapacity").value = "";

    }
    catch (err) {
        console.log(err);
    }

}


async function loadClasses() {
    var Classtable = document.getElementById("classTable");
    Classtable.innerHTML = "";


    try {
        const response = await authFetch("https://localhost:7290/api/teacher/classes");

        const classes = await response.json();

        let index = 1;
        classes.forEach(classx => {
            Classtable.innerHTML += `
            <tr>
                <td> ${index++}</td>       
                <td> ${classx.cName}</td>       
                <td style="width: 150px;"> ${classx.cCapacity}</td>                   
                <td> <button class="edit-btn" onClick="handleEdit(${classx.cid})">Edit</button>
                    <button class="delete-btn" onClick="handleEdit(${classx.cid})">Delete</button>
                </td>       
            </tr>`
        });
    }
    catch (err) {
        console.log(err);
    }

}

async function classDropdown() {

    // Adding dropdown logic for class list
    const dropdown = document.getElementById('class-select-dropdown');

    document.getElementById('class-select-dropdown').innerHTML = "";

    try {
        const response = await fetch("https://localhost:7290/api/teacher/classes");
        const classes = await response.json();


        classes.forEach(c => {
            const option = document.createElement('option');

            option.value = c.cid;
            option.textContent = c.cName;
            dropdown.appendChild(option);
        });
    }
    catch (err) {
        console.log(err);
    }
}


async function searchStudentToE() {
    var tableBody = document.getElementById("modal-student-list-table");
    var searchStudentName = document.getElementById("searchStudent").value;
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
            <tr onClick="handleEdit(${student.sId})" data-bs-dismiss="modal">
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

    document.getElementById("searchStudent").value = "";
}


function EnrollStudentSetStudent(name, sId) {
    document.getElementById("StudName_EnrollClass").value = name;
    document.getElementById("studentToEnrollId").value = sId;
}

async function EnrollStudent() {
    var sId = document.getElementById("studentToEnrollId").value;
    var cId = document.getElementById("class-select-dropdown").value;
    console.log(sId);
    console.log(cId);

    /*
    const response = await fetch("https://localhost:7290/api/teacher/enrollStudent", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            "SId": sId,
            "CId": cId
        }),
        credentials: "include"
    });
    */

    const response = await authFetch("https://localhost:7290/api/teacher/enrollStudent", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            "SId": sId,
            "CId": cId
        })
    });

    const res = await response.json();

    if (res.success) {
        iziToast.success({
            title: 'Success',
            message: res.message,
            position: 'topRight'

        });
    }
    else {
        iziToast.error({
            title: 'Error',
            message: res.message,
            position: 'topRight'

        });
    }

    document.getElementById("StudName_EnrollClass").value = "";
    document.getElementById("studentToEnrollId").value = "";
    document.getElementById("class-select-dropdown").value = "";
}


