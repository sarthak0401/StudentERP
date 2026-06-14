async function AddFeeType() {
    const feeTypeName = document.getElementById("feeTypeName").value;
    const feeTypeId = document.getElementById("hiddenField").value;

    if (feeTypeName == "") {
        iziToast.error({
            title: 'Error',
            message: 'Please enter the Fee Type Name!',
        });
        return;
    }

    try {
        const response = await fetch("https://localhost:7098/api/admin/add/feeType", {
            method: "POST",
            headers: {
                "Content-type": "application/json"
            },
            body: JSON.stringify({
                "FeeTypeId": feeTypeId != "" ? feeTypeId : null,
                "FeeTypeName": feeTypeName
            })
        });

        const res = await response.json();

        if (res.success && (feeTypeId=="")) {
            iziToast.success({
                title: 'OK',
                message: 'Successfully added Fee Type!',
            });
        }
        else if (res.success && (feeTypeId != "")) {
            iziToast.success({
                title: 'OK',
                message: 'Successfully updated Fee Type!',
            });
        }
        else {
            iziToast.success({
                title: 'ERROR',
                message: 'Failed to add Fee Type!',
            });
        }
    } catch (err) {
        console.error(err);
        iziToast.error({
            title: 'ERROR',
            message: 'Error Inserting Fee Type!',
        });
    }
    finally {
        // This will do the cleanup
        document.getElementById("feeTypeName").value = "";
        document.getElementById("hiddenField").value = "";
        showAllFeeTypes();
    }
}


async function deleteFeeType(id) {
    try {
        const response = await fetch(`https://localhost:7098/api/admin/del/feeType/${id}`, {
            method: "DELETE",
            headers: {
                "Content-type": "application/json"
            }
        });

        const res = await response.json();

        if (res.success) {
            iziToast.success({
                title: 'OK',
                message: 'Successfully deleted Fee Type!',
            });
        }
        else {
            iziToast.error({
                title: 'ERROR',
                message: 'Failed to delete Fee Type!',
            });
        }
    } catch (err) {
        console.error(err);
        iziToast.error({
            title: 'ERROR',
            message: 'Error deleting Fee Type!',
        });
    }
    finally { showAllFeeTypes(); }
}


async function showAllFeeTypes() {
    const tableBody = document.getElementById("feeTypeTableBody");
    tableBody.innerHTML = "";

    try {
        const response = await fetch("https://localhost:7098/api/admin/get/feeType",
            {
                method: "GET",
                headers: {
                    "Content-type": "application/json"
                }
            });

        const res = await response.json();

        var index = 1;
        res.feeTypes?.forEach(fee => {
            tableBody.innerHTML += `
                <tr>
                    <td>${index++} </td>
                    <td>${fee.feeTypeName}</td>
                    <td>
                       <button class="btn btn-warning btn-sm" onClick="handleFeeTypeEdit(${fee.feeTypeId}, '${fee.feeTypeName}')">Edit</button>
                       <button class="btn btn-danger btn-sm" onClick="deleteFeeType(${fee.feeTypeId})">Delete</button>
                    </td>
                </tr>
            `;
        })
    }
    catch (err) {
        console.error(err);
    }
}

function handleFeeTypeEdit(id, name) {
    document.getElementById("hiddenField").value = id;
    document.getElementById("feeTypeName").value = name;
}

// Calling this function on the startup 
showAllFeeTypes();


async function loadStandardsFeeMapSection() {
    const dropdownStd = document.getElementById("stdName");

    try {
        const response = await fetch("https://localhost:7098/api/admin/get/standards", {
            method: "GET",
            headers: {
                "Content-type": "application/json"
            }
        });

        const res = await response.json();

        console.log(res);
        res.standards?.forEach(standard => {
            const option = document.createElement("option");

            option.value = standard.standardId;
            option.textContent = `Standard - ${standard.standardName}`;

            dropdownStd.appendChild(option);
        });
    }
    catch (err) {
        console.error(err);
    }
}

loadStandardsFeeMapSection();

// Adding the event listener to this dropdown, when a particular thing is selected in it, CHANGE will happen, and that will trigger this
const stdDropdown = document.getElementById("stdName");
stdDropdown.addEventListener("change", function () {
    const selectedText = stdDropdown.options[stdDropdown.selectedIndex].text;

    document.getElementById("selectedStandardContainer").innerHTML = `
        <div class="card mt-3">
            <div class="card-body">
                <h5>${selectedText}</h5>
                <p>Select fee types for this standard.</p>
            </div>
        </div>
    `;
    document.getElementById("outerDivFeeTypes").classList.remove("visually-hidden");
});



async function feeTypesCheckBox() {
    const checkBxList = document.getElementById("feeTypesCheckBox");
    try {
        const response = await fetch("https://localhost:7098/api/admin/get/feeType", {
            method: "GET",
            headers: {
                "Content-type": "application/json"
            }
        });

        const res = await response.json();

        checkBxList.innerHTML = "";

            res.feeTypes.forEach(fee => {
                checkBxList.innerHTML += `
                <div class="form-check mb-2">
                    <input class="form-check-input fee-checkbox"
                           type="checkbox"
                           value="${fee.feeTypeId}"
                           id="fee_${fee.feeTypeId}"
                            >

                    <label class="form-check-label"
                           for="fee_${fee.feeTypeId}">
                        ${fee.feeTypeName}
                    </label>
                </div>
            `;
            })
    }
    catch {
        console.error(err);
    }
}

feeTypesCheckBox();


async function addStdFeeMappingBtn() {
    const stdId = document.getElementById("stdName").value;
    const feeTypes = [];

    document.querySelectorAll(".fee-checkbox:checked").forEach(fee => {
        feeTypes.push(parseInt(fee.value));
    });

    try {
        const response = await fetch("https://localhost:7098/api/admin/add/stdFeeMap", {
            method: "POST",
            headers: {
                "Content-type": "application/json"
            },
            body: JSON.stringify({
                "StdId": parseInt(stdId),
                "FeeTypes": feeTypes
            })
        });

        const res = await response.json();
        if (res.success) {
            iziToast.success({
                title: "OK!",
                message: "Mapping done successfully!"
            });
        }
        else {
            iziToast.error({
                title: "ERROR!",
                message: "Mapping failed!"
            });
        }
    }
    catch (err) {
        console.error(err);
        iziToast.error({
            title: "ERROR!",
            message: "Server Error!"
        });
    }
    finally {
        // Resetting the dropdown 
        document.getElementById("stdName").selectedIndex = 0;

        // resetting the checkboxes
        document.querySelectorAll(".fee-checkbox").forEach(ch => {
            ch.checked = false;
        })

        // removing the card which was showing the name of the standard
        document.getElementById("selectedStandardContainer").innerHTML = "";


        // Hiding the div which was showing all the available fee types 
        document.getElementById("outerDivFeeTypes").classList.add("visually-hidden");
    }
}

