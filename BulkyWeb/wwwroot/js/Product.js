var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { "url": "/Admin/Product/GetAll" },
        "columns": [
            { "data": "title", "width": "18%" },
            { "data": "isbn", "width": "12%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "12%" },
            {
                "data": "price",
                "width": "8%",
                render: function (data) {
                    return `$${parseFloat(data).toFixed(2)}`;
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group d-flex justify-content-center" role="group" style="gap: 0.4rem;">
                            <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary btn-sm px-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <button onclick="Delete('/Admin/Product/Delete/${data}')" class="btn btn-danger btn-sm px-2">
                                <i class="bi bi-trash-fill"></i> Delete
                            </button>
                        </div>`;
                },
                "width": "12%",
                "orderable": false,
                "searchable": false
            }
        ],
        "width": "100%",
        "autoWidth": false,
        "responsive": true,
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "No products found."
        }
    });
}




function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Delete failed:", xhr, status, error);
                    console.log("Response text:", xhr.responseText);
                    // show human friendly message
                    if (xhr.status === 401 || xhr.status === 403) {
                        toastr.error("Unauthorized. Please login as admin.");
                    } else {
                        toastr.error("Server error while deleting. See console for details.");
                    }
                }
            });
        }
    });
}

