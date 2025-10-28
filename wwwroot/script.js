const apiBase = "/api/employees";
let editingId = null;

async function loadEmployees() {
  const res = await fetch(apiBase);
  const data = await res.json();
  const table = document.getElementById("employeeTable");
  table.innerHTML = "";

  data.forEach(emp => {
    const row = document.createElement("tr");
    row.innerHTML = `
      <td>${emp.id}</td>
      <td>${emp.name}</td>
      <td>${emp.department ?? "N/A"}</td>
      <td>${emp.salaryPerDay}</td>
      <td>
        <button class="action-btn edit" onclick="editEmployee(${emp.id}, '${emp.name}', '${emp.department}', ${emp.salaryPerDay})">Edit</button>
        <button class="action-btn" onclick="deleteEmployee(${emp.id})">Delete</button>
      </td>
    `;
    table.appendChild(row);
  });
}

async function addEmployee() {
  const name = document.getElementById("name").value;
  const department = document.getElementById("department").value;
  const salaryPerDay = parseInt(document.getElementById("salary").value);

  await fetch(apiBase, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name, department, salaryPerDay })
  });

  clearForm();
  loadEmployees();
}

async function deleteEmployee(id) {
  if (!confirm("Are you sure you want to delete this employee?")) return;
  await fetch(`${apiBase}/${id}`, { method: "DELETE" });
  loadEmployees();
}

function editEmployee(id, name, department, salary) {
  document.getElementById("name").value = name;
  document.getElementById("department").value = department;
  document.getElementById("salary").value = salary;
  editingId = id;

  document.getElementById("addBtn").style.display = "none";
  document.getElementById("updateBtn").style.display = "inline-block";
}

async function updateEmployee() {
  const name = document.getElementById("name").value;
  const department = document.getElementById("department").value;
  const salaryPerDay = parseInt(document.getElementById("salary").value);

  await fetch(`${apiBase}/${editingId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ id: editingId, name, department, salaryPerDay })
  });

  clearForm();
  loadEmployees();
}

function clearForm() {
  document.getElementById("name").value = "";
  document.getElementById("department").value = "";
  document.getElementById("salary").value = "";
  document.getElementById("addBtn").style.display = "inline-block";
  document.getElementById("updateBtn").style.display = "none";
  editingId = null;
}

document.getElementById("addBtn").addEventListener("click", addEmployee);
document.getElementById("updateBtn").addEventListener("click", updateEmployee);

loadEmployees();
