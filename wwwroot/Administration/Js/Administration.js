// ============================================================
//  Administration.js
//  Wire each function below to your ASP.NET Core API endpoints
// ============================================================

// ── Add Student ─────────────────────────────────────────────

async function SubmitStudentDetails() {
    const studentId = document.getElementById('HiddenStudentId').value;
    const isEdit = studentId !== '';

    const contacts = [...document.querySelectorAll('.contact-number')]
        .map(i => i.value.trim()).filter(Boolean);

    const gender = document.querySelector('input[name="sgender"]:checked');

    const payload = {
        id: studentId || 0,
        name: document.getElementById('sname').value.trim(),
        dob: document.getElementById('sdob').value,
        contacts: contacts,
        gender: gender ? gender.value : '',
        bloodGroup: document.getElementById('sbloodGrp').value.trim(),
        fatherName: document.getElementById('fname').value.trim(),
        motherName: document.getElementById('mname').value.trim(),
        guardianContact1: document.getElementById('gcontact1').value.trim(),
        guardianContact2: document.getElementById('gcontact2').value.trim(),
        address: document.getElementById('address').value.trim(),
        city: document.getElementById('city').value.trim(),
        state: document.getElementById('state').value.trim(),
        pincode: document.getElementById('pincode').value.trim(),
        tenthPercentage: document.getElementById('10thPerctg').value,
        twelfthPercentage: document.getElementById('12thPerctg').value,
        jeeRank: document.getElementById('jeeRank').value,
        schoolName: document.getElementById('schoolName').value.trim(),
        gapYears: document.getElementById('gapYrs').value,
        gapJustification: document.getElementById('gapYrJustification').value.trim(),
    };

    // Handle image
    const fileInput = document.getElementById('sPhotoUpload');
    if (fileInput.files.length > 0) {
        payload.image = await toBase64(fileInput.files[0]);
    } else {
        payload.image = document.getElementById('existingImage').value || '';
    }

    try {
        // TODO: replace URL with your actual endpoint
        const url = isEdit ? `/api/Student/Update` : `/api/Student/Add`;
        const method = isEdit ? 'PUT' : 'POST';

        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload),
        });

        if (!res.ok) throw new Error(await res.text());

        iziToast.success({ title: 'Success', message: isEdit ? 'Student updated!' : 'Student added!' });
        clearStudentForm();
        showSection('student-list-section');
        loadStudents();
    } catch (err) {
        iziToast.error({ title: 'Error', message: err.message });
    }
}

function clearStudentForm() {
    ['sname', 'sdob', 'sbloodGrp', 'fname', 'mname', 'gcontact1', 'gcontact2',
        'address', 'city', 'state', 'pincode', '10thPerctg', '12thPerctg',
        'jeeRank', 'schoolName', 'gapYrs', 'gapYrJustification', 'HiddenStudentId', 'existingImage']
        .forEach(id => { const el = document.getElementById(id); if (el) el.value = ''; });

    document.querySelectorAll('input[name="sgender"]').forEach(r => r.checked = false);
    document.getElementById('sPhotoUpload').value = '';

    const preview = document.getElementById('studentImagePreviewContainer');
    if (preview) preview.classList.add('d-none');

    // Reset contact rows to one empty row
    const container = document.getElementById('contactContainer');
    container.innerHTML = `
        <div class="mb-2 contact-row d-flex gap-2">
            <input type="text" class="form-control contact-number" placeholder="Enter Contact Number" maxlength="10">
            <button type="button" class="btn btn-success" onclick="addContact()">+</button>
        </div>`;
}

// ── Student List ─────────────────────────────────────────────
/*
async function loadStudents(fromDate = '', toDate = '', search = '') {
    try {
        const params = new URLSearchParams();
        if (fromDate) params.append('from', fromDate);
        if (toDate) params.append('to', toDate);
        if (search) params.append('q', search);

        // TODO: replace with your actual endpoint
        const res = await fetch(`/api/Student/List?${params.toString()}`);
        const data = await res.json();
        renderStudentTable(data);
    } catch (err) {
        iziToast.error({ title: 'Error', message: 'Failed to load students.' });
    }
}
*/


function renderStudentTable(students) {
    const tbody = document.getElementById('student-table-body');
    if (!students || students.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center py-4 text-muted">No students found</td></tr>`;
        return;
    }
    tbody.innerHTML = students.map((s, i) => `
        <tr>
            <td>${i + 1}</td>
            <td>${s.name}</td>
            <td>${s.fatherName}</td>
            <td>${s.motherName}</td>
            <td>${s.address}, ${s.city}</td>
            <td>
                <button class="btn btn-warning btn-sm me-1 actionCst-btn" onclick="editStudent(${s.id})">
                    <i class="bi bi-pencil-fill"></i> Edit
                </button>
                <button class="btn btn-danger btn-sm actionCst-btn" onclick="deleteStudent(${s.id})">
                    <i class="bi bi-trash-fill"></i> Delete
                </button>
            </td>
        </tr>`).join('');
}

function searchStudent() {
    const from = document.getElementById('fromDate').value;
    const to = document.getElementById('toDate').value;
    const search = document.getElementById('searchBox').value.trim();
    loadStudents(from, to, search);
}

/*
async function editStudent(id) {
    try {
        // TODO: replace with your actual endpoint
        const res = await fetch(`/api/Student/${id}`);
        const data = await res.json();
        populateStudentForm(data);
        showSection('add-student-section');
    } catch (err) {
        iziToast.error({ title: 'Error', message: 'Failed to load student details.' });
    }
}
*/

function populateStudentForm(s) {
    document.getElementById('HiddenStudentId').value = s.id;
    document.getElementById('sname').value = s.name || '';
    document.getElementById('sdob').value = s.dob || '';
    document.getElementById('sbloodGrp').value = s.bloodGroup || '';
    document.getElementById('fname').value = s.fatherName || '';
    document.getElementById('mname').value = s.motherName || '';
    document.getElementById('gcontact1').value = s.guardianContact1 || '';
    document.getElementById('gcontact2').value = s.guardianContact2 || '';
    document.getElementById('address').value = s.address || '';
    document.getElementById('city').value = s.city || '';
    document.getElementById('state').value = s.state || '';
    document.getElementById('pincode').value = s.pincode || '';
    document.getElementById('10thPerctg').value = s.tenthPercentage || '';
    document.getElementById('12thPerctg').value = s.twelfthPercentage || '';
    document.getElementById('jeeRank').value = s.jeeRank || '';
    document.getElementById('schoolName').value = s.schoolName || '';
    document.getElementById('gapYrs').value = s.gapYears || '';
    document.getElementById('gapYrJustification').value = s.gapJustification || '';

    if (s.gender) {
        const radio = document.querySelector(`input[name="sgender"][value="${s.gender}"]`);
        if (radio) radio.checked = true;
    }

    if (s.image) {
        document.getElementById('existingImage').value = s.image;
        document.getElementById('studentImagePreview').src = s.image;
        document.getElementById('studentImagePreviewContainer').classList.remove('d-none');
    }
}

async function deleteStudent(id) {
    if (!confirm('Are you sure you want to delete this student?')) return;
    try {
        // TODO: replace with your actual endpoint
        await fetch(`/api/Student/Delete/${id}`, { method: 'DELETE' });
        iziToast.success({ title: 'Deleted', message: 'Student deleted successfully.' });
        loadStudents();
    } catch (err) {
        iziToast.error({ title: 'Error', message: 'Failed to delete student.' });
    }
}

// ── Fee Receipts ─────────────────────────────────────────────

async function loadFeeReceipts() {
    try {
        // TODO: replace with your actual endpoint
        const res = await fetch('/api/FeeReceipt/List');
        const data = await res.json();
        renderFeeReceiptsTable(data);
    } catch (err) {
        iziToast.error({ title: 'Error', message: 'Failed to load receipts.' });
    }
}

function renderFeeReceiptsTable(receipts) {
    const tbody = document.getElementById('feeReceiptsTableBody');
    if (!receipts || receipts.length === 0) {
        tbody.innerHTML = `<tr><td colspan="8" class="text-center text-muted py-4">No receipts found</td></tr>`;
        return;
    }
    tbody.innerHTML = receipts.map(r => `
        <tr>
            <td><strong>${r.receiptNo}</strong></td>
            <td>${r.studentName}</td>
            <td>${r.feeType}</td>
            <td>₹${r.amount}</td>
            <td>${r.paymentMode}</td>
            <td>${formatDate(r.date)}</td>
            <td><span class="badge ${r.status === 'Active' ? 'bg-success' : 'bg-danger'}">${r.status}</span></td>
            <td>
                <button class="btn btn-sm btn-outline-primary me-1" onclick="printReceipt('${r.receiptNo}')">
                    <i class="bi bi-printer"></i>
                </button>
                ${r.status === 'Active' ? `
                <button class="btn btn-sm btn-outline-danger" onclick="openCancelModal('${r.receiptNo}')">
                    <i class="bi bi-x-circle"></i>
                </button>` : ''}
            </td>
        </tr>`).join('');
}

function printReceipt(receiptNo) {
    // TODO: open print view / PDF for receipt
    iziToast.info({ title: 'Print', message: `Printing receipt ${receiptNo}` });
}

// ── Cancel Receipts ──────────────────────────────────────────

async function searchReceiptToCancel() {
    const receiptNo = document.getElementById('cancelReceiptNo').value.trim();
    const studentName = document.getElementById('cancelStudentName').value.trim();

    if (!receiptNo && !studentName) {
        iziToast.warning({ title: 'Warning', message: 'Enter a receipt number or student name.' });
        return;
    }

    try {
        const params = new URLSearchParams();
        if (receiptNo) params.append('receiptNo', receiptNo);
        if (studentName) params.append('student', studentName);

        // TODO: replace with your actual endpoint
        const res = await fetch(`/api/FeeReceipt/Search?${params.toString()}`);
        const data = await res.json();
        renderCancelSearchResults(data);
    } catch (err) {
        iziToast.error({ title: 'Error', message: 'Search failed.' });
    }
}

function renderCancelSearchResults(receipts) {
    const tbody = document.getElementById('cancelReceiptsTableBody');
    if (!receipts || receipts.length === 0) {
        tbody.innerHTML = `<tr><td colspan="8" class="text-center text-muted py-4">No matching receipts found</td></tr>`;
        return;
    }
    tbody.innerHTML = receipts.map(r => `
        <tr>
            <td><strong>${r.receiptNo}</strong></td>
            <td>${r.studentName}</td>
            <td>${r.feeType}</td>
            <td>₹${r.amount}</td>
            <td>${r.paymentMode}</td>
            <td>${formatDate(r.date)}</td>
            <td><span class="badge ${r.status === 'Active' ? 'bg-success' : 'bg-danger'}">${r.status}</span></td>
            <td>
                ${r.status === 'Active' ? `
                <button class="btn btn-sm btn-danger" onclick="openCancelModal('${r.receiptNo}')">
                    <i class="bi bi-x-circle me-1"></i>Cancel
                </button>` : '<span class="text-muted">Already Cancelled</span>'}
            </td>
        </tr>`).join('');
}

async function confirmCancelReceipt() {
    const reason = document.getElementById('cancelReason').value.trim();
    if (!reason) { iziToast.warning({ title: 'Warning', message: 'Please enter a reason.' }); return; }

    try {
        // TODO: replace with your actual endpoint
        await fetch(`/api/FeeReceipt/Cancel`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ receiptNo: receiptToCancel, reason }),
        });

        bootstrap.Modal.getInstance(document.getElementById('cancelReceiptModal')).hide();
        iziToast.success({ title: 'Cancelled', message: `Receipt ${receiptToCancel} cancelled.` });
        document.getElementById('cancelReason').value = '';
        receiptToCancel = null;
        loadCancelledLog();
        searchReceiptToCancel();
    } catch (err) {
        iziToast.error({ title: 'Error', message: 'Failed to cancel receipt.' });
    }
}

async function loadCancelledLog() {
    try {
        // TODO: replace with your actual endpoint
        const res = await fetch('/api/FeeReceipt/CancelledLog');
        const data = await res.json();
        const tbody = document.getElementById('cancelledLogTableBody');
        if (!data || data.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted py-4">No cancelled receipts</td></tr>`;
            return;
        }
        tbody.innerHTML = data.map(r => `
            <tr>
                <td><strong>${r.receiptNo}</strong></td>
                <td>${r.studentName}</td>
                <td>₹${r.amount}</td>
                <td>${formatDate(r.cancelledOn)}</td>
                <td>${r.reason}</td>
            </tr>`).join('');
    } catch (_) { }
}

// ── Utilities ────────────────────────────────────────────────

function toBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
        reader.readAsDataURL(file);
    });
}

function formatDate(dateStr) {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-IN');
}

// ── On page load ─────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    loadStudents();
    loadFeeReceipts();
    loadCancelledLog();
});