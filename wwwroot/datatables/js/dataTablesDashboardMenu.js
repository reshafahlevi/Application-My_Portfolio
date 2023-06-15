$(document).ready(function () {

    var DT = $("#DataTablesDashboardMenu").DataTable
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
                "url": "/Dashboard/LoadData",
                "type": "POST",
                "datatype": "JSON"
            },
            "columnDefs":
                [{
                    "targets": [0],
                    "visible": true,
                    "searchable": true,
                    "className": "SelectedAllCheckbox",
                    "checkboxes":
                    {
                        "selectRow": true,
                        "selectAllPages": false
                    },
                }],
            "select":
            {
                "style": "multi",
                "selector": "td:first-child"
            },
            "order": [[0, "asc"]],
            "columns":
                [
                    {
                        "data": "id",
                        "name": "id",
                        "autoWidth": true, "target": 0, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "kodeMenu",
                        "name": "kodeMenu",
                        "autoWidth": true, "target": 1, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "namaMenu",
                        "name": "namaMenu",
                        "autoWidth": true, "target": 2, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "jenisMenu",
                        "name": "jenisMenu",
                        "autoWidth": true, "target": 3, "visible": true, "searchable": true, "orderable": true
                    },
                    {
                        "data": "hargaSatuan",
                        "name": "hargaSatuan",
                        "autoWidth": true, "target": 4, "visible": true, "searchable": true, "orderable": true
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

    $("#TextboxSearchSidebar").keyup(function () {
        DT.search($(this).val()).draw();
    });

});

function GetDetailMenuById(jenisMenu, kodeMenu, namaMenu, hargaSatuan) {
    //alert("Menu yang anda pilih adalah Id nomor : " + id);
    console.log(jenisMenu, kodeMenu, namaMenu, hargaSatuan);
    $("#PopUpDetailItem").modal('show');
    //$("#TextboxId").val(id);
    $("#TextboxJenisMenu").val(jenisMenu);
    $("#TextboxKodeMenu").val(kodeMenu);
    $("#TextboxNamaMenu").val(namaMenu);
    $("#TextboxHargaSatuan").val(hargaSatuan);
    /*------------------------------------------------------------------------*/
    //var Id = $("#TextboxId").val(id);
    //var SendData = { JenisMenu: JenisMenu };

    //var SendData =
    //{
    //    Id: id
    //}
    //$.ajax({
    //    type: "GET",
    //    url: "/Dashboard/GetDataById/" + id,
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "JSON",
    //    data: SendData, //JSON.stringify(SendData),
    //    success: function (response)
    //    {
    //        if (response != null)
    //        {
    //            //alert("Menu yang anda pilih adalah Id nomor : " + id);
    //            console.log(jenisMenu, kodeMenu, namaMenu, qty, hargaSatuan);
    //            $("#PopUpDetailItem").modal('show')
    //            //$("#TextboxId").val(id);
    //            $("#TextboxJenisMenu").val(jenisMenu);
    //            $("#TextboxKodeMenu").val(kodeMenu);
    //            $("#TextboxNamaMenu").val(namaMenu);
    //            $("#TextboxHargaSatuan").val(hargaSatuan);
    //        }
    //        else
    //        {
    //            alert("Something went wrong !");
    //        }
    //    },
    //    failure: function (response)
    //    {
    //        alert(response.responseText);
    //    },
    //    error: function (response)
    //    {
    //        alert(response.responseText);
    //    }
    //});
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
    var url = "/Dashboard/DeleteData";
    $.post(url, { ID: id }, function (data) {
        if (data) {
            oTable = $('#DataTablesDashboardMenu').DataTable();
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

function OpenSidebar() {
    document.getElementById("Main").style.marginLeft = "100px";
}

function HideSideBar() {
    document.getElementById("Main").style.marginLeft = "0px";
}