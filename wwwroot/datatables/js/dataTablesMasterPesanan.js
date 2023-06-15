$(document).ready(function () {
    var DT = $("#DataTablesMasterPesanan").DataTable
        ({
            //"scrollY": 100,
            //"scrollX": true,
            //"scrollCollapse": true,
            //"scrollResize": true,
            //"responsive": true,
            //"processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "orderMulti": true, // for disable multiple column at once
            "fixedHeader": true, // this is fixed header
            "pagingType": "full_numbers", // this is for paging type
            "lengthChange": true, // this is for length change
            "lengthMenu":  // this is for length dropdown
                [
                    [5, 10, 25, 50, 100, 1000],
                    [5, 10, 25, 50, 100, 1000],
                ],
            "ajax":
            {
                "url": "/MasterPesanan/LoadData",
                "type": "POST",
                "datatype": "JSON"
            },
            "columnDefs":
                [{
                    "targets": [1],
                    "visible": true,
                    "searchable": true,
                    "className": "SelectedAllCheckbox",
                    "checkboxes": {
                        "selectRow": true,
                        "selectAllPages": false
                    },
                }],
            "select":
            {
                "style": "multi",
                "selector": "td:first-child"
            },
            "order": [[1, "asc"]],
            "columns":
                [
                    {
                        "targets": 0,
                        "data": null,
                        "className": "SelectedAllCheckbox",
                        "searchable": false,
                        "orderable": false,
                        "autoWidth": false,
                        "render": function (data, row, type, full, meta) {
                            return '<input type="checkbox" class="SelectedAllCheckbox" name="HeaderAllCheckbox" id="HeaderAllCheckbox" ClientIDMode="Static" value="' + data.id + '" onclick=GetById("' + data.id + '")>';
                            ////return "<input type='checkbox class='SelectedAllCheckbox' name='HeaderAllCheckbox' id='HeaderAllCheckbox' ClientIDMode='Static' onclick=GetById('" + data.id + "')";
                        },
                    },
                    {
                        "data": "id",
                        "name": "id",
                        "autoWidth": true, "target": 1, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "kodePemesanan",
                        "name": "kodePemesanan",
                        "autoWidth": true, "target": 2, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "namaPemesan",
                        "name": "namaPemesan",
                        "autoWidth": true, "target": 3, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "kodeMenu",
                        "name": "kodeMenu",
                        "autoWidth": true, "target": 4, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "namaMenu",
                        "name": "namaMenu",
                        "autoWidth": true, "target": 5, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "jumlahPesanan",
                        "name": "jumlahPesanan",
                        "autoWidth": true, "target": 6, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "hargaSatuan",
                        "name": "hargaSatuan",
                        "autoWidth": true, "target": 7, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": null,
                        "name": null,
                        "autoWidth": true, "target": 5, "visible": true, "searchable": false, "orderable": false, "render": function (data, row, type, full, meta) {
                            return "<a href='#PopUpDetailItem' data-toggle='modal' data-target='#PopUpDetailItem' id='ButtonGetById' ClientIDMode='Static' class='css-button-custome_details' onclick='GetDetailMenuById(\"" + data.jenisMenu + "\",\"" + data.kodeMenu + "\",\"" + data.namaMenu + "\",\"" + data.hargaSatuan + "\");'><span class='css-button-custome_details-icon'><i class='fa fa-eye'></i></span><span class='css-button-custome_details-text'><span>Details</span></span></a>";  //onclick='GetDetailMenuById(" + data.id + ",\"" + data.kodeMenu + "\",\"" + data.namaMenu + "\",\"" + data.jenisMenu + "\",\"" + data.qty + "\");
                        }
                    }
                    //{
                    //    "data": null,
                    //    "name": null,
                    //    "autoWidth": true, "target": 5, "visible": true, "searchable": false, "orderable": false, "render": function (data, row, type, full, meta)
                    //    {
                    //        return "<a href='#PopUpDetailItem' data-toggle='modal' data-target='#PopUpDetailItem' id='ButtonGetById' ClientIDMode='Static' class='btn btn-info dataTables-buttonstyle' style='width: 100% !important;' onclick='GetDetailMenuById(\"" + data.jenisMenu + "\",\"" + data.kodeMenu + "\",\"" + data.namaMenu + "\",\"" + data.hargaSatuan + "\");'>Details</a>";  //onclick='GetDetailMenuById(" + data.id + ",\"" + data.kodeMenu + "\",\"" + data.namaMenu + "\",\"" + data.jenisMenu + "\",\"" + data.qty + "\");
                    //    }
                    //}
                    ////{
                    ////    "data": null,"target": 6, "render": function (data, row, type, full, meta)
                    ////    {
                    ////        return "<a href='#' id='ButtonGetById' ClientIDMode='Static' class='btn btn-info dataTables-buttonstyle' style='width: 45% !important;' onclick=GetById('" + row.id + "');>Info</a> | <a href='#' id='ButtonDeleteById' ClientIDMode='Static' class='btn btn-danger dataTables-buttonstyle' style='width: 45% !important;' onclick=DeleteData('" + row.id + "');>Delete</a>";
                    ////    }
                    ////}
                ]
        });

    /*Benar------------------------------------------*/
    $('#HeaderAllCheckbox').click(function (e) {
        if ($(this).hasClass('SelectedAllCheckbox')) {
            $('input').prop('checked', false);
            $(this).removeClass('SelectedAllCheckbox');
        }
        else {
            $('input').prop('checked', true);
            $(this).addClass('SelectedAllCheckbox');
        }
    });
    /*----------------------------------------------*/
});

function GetDetailMenuById(kodePemesanan, namaPemesan, kodeMenu, namaMenu, jumlahPesanan, hargaSatuan) {
    /*------------------------------------------------------------------------*/

    //alert("Menu yang anda pilih adalah Id nomor : " + id);
    console.log(kodeMenu, namaMenu, jenisMenu, qty);
    $("#PopUpDetailItem").modal('show')
    //$("#TextboxId").val(id);
    $("#TextboxKodePemesanan").val(kodePemesanan);
    $("#TextboxNamaPemesan").val(namaPemesan);
    $("#TextboxKodeMenu").val(kodeMenu);
    $("#TextboxNamaMenu").val(namaMenu);
    $("#TextboxJumlahPesanan").val(jumlahPesanan);
    $("#TextboxHargaSatuan").val(hargaSatuan);

    /*------------------------------------------------------------------------*/
}

function GetById(id) {
    alert("Menu yang anda pilih adalah Id nomor : " + id);
    console.log(id);
}

function DeleteData(id) {
    if (confirm("Are you sure you want to delete ...?")) {
        Delete(id);
        console.log(id);
    }
    else {
        return false;
    }
}

function Delete(id) {
    var url = "/MasterPesanan/DeleteData";
    $.post(url, { ID: id }, function (data) {
        if (data) {
            oTable = $('#DataTablesMasterPesanan').DataTable();
            oTable.draw();
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}

function ClosePopUpModal() {
    $("#PopUpDetailItem").modal('hide')
}