// ============================================================
//  AddStudent.js  –  wwwroot/Administration/Js/AddStudent.js
//  Wire the TODO fetch calls to your ASP.NET Core API
// ============================================================

// ── Submit (Add or Edit) ─────────────────────────────────────

async function SubmitStudentDetails() {
    const studentId = document.getElementById('HiddenStudentId').value;
    const isEdit = studentId !== '';

    // Basic validation
    if (!validateForm()) return;

    const contacts = [...document.querySelectorAll('.contact-number')]
        .map(i => i.value.trim()).filter(Boolean);

    const gender = document.querySelector('input[name="sgender"]:checked');

    const payload = {
        id: isEdit ? parseInt(studentId) : 0,
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
        tenthPercentage: parseFloat(document.getElementById('10thPerctg').value) || 0,
        twelfthPercentage: parseFloat(document.getElementById('12thPerctg').value) || 0,
        jeeRank: parseInt(document.getElementById('jeeRank').value) || 0,
        schoolName: document.getElementById('schoolName').value.trim(),
        gapYears: parseInt(document.getElementById('gapYrs').value) || 0,
        gapJustification: document.getElementById('gapYrJustification').value.trim(),
    };

    // Image handling
    const fileInput = document.getElementById('sPhotoUpload');
    if (fileInput.files.length > 0) {
        payload.image = await toBase64(fileInput.files[0]);
    } else {
        payload.image = document.getElementById('existingImage').value || '';
    }

    try {
        // TODO: replace with your actual API endpoints
        const url = isEdit ? `/api/Student/Update` : `/api/Student/Add`;
        const method = isEdit ? 'PUT' : 'POST';

        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload),
        });

        if (!res.ok) {
            const errText = await res.text();
            throw new Error(errText || 'Request failed');
        }

        iziToast.success({
            title: 'Success',
            message: isEdit ? 'Student updated successfully!' : 'Student added successfully!',
        });

        clearStudentForm();

        // Redirect to student list after short delay
        setTimeout(() => { window.location.href = 'StudentList.html'; }, 1200);

    } catch (err) {
        iziToast.error({ title: 'Error', message: err.message });
    }
}

// ── Validation ───────────────────────────────────────────────

function validateForm() {
    const name = document.getElementById('sname').value.trim();
    const dob = document.getElementById('sdob').value;
    const fname = document.getElementById('fname').value.trim();
    const city = document.getElementById('city').value.trim();

    if (!name) {
        iziToast.warning({ title: 'Validation', message: 'Student name is required.' });
        switchTab('personal-info-tab');
        return false;
    }
    if (!dob) {
        iziToast.warning({ title: 'Validation', message: 'Date of birth is required.' });
        switchTab('personal-info-tab');
        return false;
    }
    if (!fname) {
        iziToast.warning({ title: 'Validation', message: "Father's name is required." });
        switchTab('parent-details-tab');
        return false;
    }
    if (!city) {
        iziToast.warning({ title: 'Validation', message: 'City is required.' });
        switchTab('address-tab');
        return false;
    }

    const contacts = [...document.querySelectorAll('.contact-number')]
        .map(i => i.value.trim()).filter(Boolean);
    if (contacts.length === 0 || contacts[0].length !== 10) {
        iziToast.warning({ title: 'Validation', message: 'Enter a valid 10-digit contact number.' });
        switchTab('personal-info-tab');
        return false;
    }

    return true;
}

// ── Populate form for Edit ────────────────────────────────────

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

    // Gender
    if (s.gender) {
        const radio = document.querySelector(`input[name="sgender"][value="${s.gender}"]`);
        if (radio) radio.checked = true;
    }

    // Contact numbers — rebuild rows
    if (s.contacts && s.contacts.length > 0) {
        const container = document.getElementById('contactContainer');
        container.innerHTML = '';
        s.contacts.forEach((num, idx) => {
            const div = document.createElement('div');
            div.className = 'mb-2 contact-row d-flex gap-2';
            div.innerHTML = `
                <input type="text" class="form-control contact-number"
                       placeholder="Enter Contact Number" maxlength="10" value="${num}">
                ${idx === 0
                    ? `<button type="button" class="btn btn-success" onclick="addContact()">+</button>`
                    : `<button type="button" class="btn btn-danger" onclick="this.parentElement.remove()">−</button>`
                }`;
            container.appendChild(div);
        });
    }

    // Image
    if (s.image) {
        document.getElementById('existingImage').value = s.image;
        document.getElementById('studentImagePreview').src = s.image;
        document.getElementById('studentImagePreviewContainer').classList.remove('d-none');
    }
}

// ── Clear form ────────────────────────────────────────────────

function clearStudentForm() {
    const fields = [
        'sname', 'sdob', 'sbloodGrp',
        'fname', 'mname', 'gcontact1', 'gcontact2',
        'address', 'city', 'state', 'pincode',
        '10thPerctg', '12thPerctg', 'jeeRank',
        'schoolName', 'gapYrs', 'gapYrJustification',
        'HiddenStudentId', 'existingImage',
    ];
    fields.forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = '';
    });

    document.querySelectorAll('input[name="sgender"]').forEach(r => r.checked = false);
    document.getElementById('sPhotoUpload').value = '';
    document.getElementById('studentImagePreviewContainer').classList.add('d-none');
    document.getElementById('studentImagePreview').src = '';

    // Reset contact rows to one empty row
    document.getElementById('contactContainer').innerHTML = `
        <div class="mb-2 contact-row d-flex gap-2">
            <input type="text" class="form-control contact-number"
                   placeholder="Enter Contact Number" maxlength="10">
            <button type="button" class="btn btn-success" onclick="addContact()">+</button>
        </div>`;

    // Reset to first tab and step
    switchTab('personal-info-tab');
    updateStepIndicator(1);
}

// ── Utility: File → Base64 ────────────────────────────────────

function toBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
        reader.readAsDataURL(file);
    });
}

// ── On load: check for edit mode via query param ──────────────
// Usage: AddStudent.html?id=42  →  auto-loads and populates form

document.addEventListener('DOMContentLoaded', async () => {
    const params = new URLSearchParams(window.location.search);
    const editId = params.get('id');

    if (editId) {
        try {
            // TODO: replace with your actual endpoint
            const res = await fetch(`/api/Student/${editId}`);
            if (!res.ok) throw new Error('Student not found');
            const data = await res.json();
            populateStudentForm(data);

            // Update page heading to reflect edit mode
            const heading = document.querySelector('.heading');
            if (heading) heading.textContent = 'Edit Student';
        } catch (err) {
            iziToast.error({ title: 'Error', message: err.message });
        }
    }
});