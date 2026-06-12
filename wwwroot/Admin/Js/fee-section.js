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