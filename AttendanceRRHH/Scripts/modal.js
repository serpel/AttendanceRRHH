function CreateTable(id, columns, source, controller) {
    var table = $(id).DataTable({
        ajax: {
            url: source,
            dataSrc: '',
            stateSave: true
        },
        columns: columns,
        columnDefs: [{
            render: function (data, type, row) {
                var editUrl = '/' + controller + '/edit/' + data;
                var deleteUrl = '/' + controller + '/delete/' + data;
                var txt = "<div class='pull-right'>" +
                    "<a class='btn btn-default' data-modal='' href='" + editUrl + "' title='Edit'><span class='glyphicon glyphicon-pencil'></span></a>&nbsp;" +
                    "<a class='btn btn-danger' data-modal='' href='" + deleteUrl + "' title='Delete'><span class='glyphicon glyphicon-trash'></span></a>" +
                    "</div>";

                return txt;
            },
            targets: -1,
            orderable: false,
            searchable: false
        }]
    });

    return table;
};

function ReloadTable(id, columns, source, controller) {
    //table.ajax.reload();
    d = $(id).DataTable().destroy();
    d = CreateTable(id, columns, source, controller);
};

function LoadModal(id, columns, source, controller) {
    $("a[data-modal]").on("click", function (e) {
        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                keyboard: true
            }, 'show');
            bindForm(this, id, columns, source, controller);
        });
        return false;
    });
};

function bindForm(dialog, id, columns, source, controller) {
    $('form', dialog).submit(function () {
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#myModal').modal('hide');
                    ReloadTable(id, columns, source, controller);

                } else {
                    $('#myModalContent').html(result);
                    bindForm(dialog, id, columns, source, controller);
                }
            }
        });
        return false;
    });
};