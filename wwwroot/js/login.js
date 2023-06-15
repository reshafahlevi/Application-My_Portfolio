$(document).ready(function () {
	window.onload = disablePrev();
	window.history.forward();
	Automatic_LogOut();
});

function SignIn() {
	var Username = $("#TextUsernameLogin").val();
	var Password = $("#TextPasswordLogin").val();
	var HakAkses = $('#TextboxHakAkses option:selected').val();
	var Object_Login =
	{
		Username: Username,
		Password: Password,
		HakAkses: HakAkses,
	};

	$.ajax
		({
			//url: "/Home/Login/", body:
			//{
			//  Username: Username,
			//  Password: Password,
			//  HakAkses: HakAkses
			//},
			url: "/Home/Login",
			type: "POST",
			contentType: "application/json; charset=utf-8",
			datatype: "JSON",
			data: JSON.stringify(Object_Login),
			type: "POST",
			contentType: "application/json; charset=utf-8",
			datatype: "JSON",
			success: function (result) {
				if (result.getStatus == 1) {
					//alert("Successful Login !");
					$("#PopUpLogin").modal('show');
					document.getElementById('LabelInformationLogin').innerHTML = 'Successful Login !';
					document.getElementById("LabelInformationLogin").style.color = "#0013c6";
					//window.location.href = "/Dashboard/Dashboard/";
					Success_RedirectPage();
				}
				else {
					//alert("Failed Login !");
					$("#PopUpLogin").modal('show');
					document.getElementById('LabelInformationLogin').innerHTML = 'Failed Login !';
					document.getElementById("LabelInformationLogin").style.color = "Red";
					Failed_RedirectPage();
				}
			},
			error: function (errormessage) {
				alert(errormessage.responseText);
			}
		});
}

function Failed_RedirectPage() {
	setInterval(Page_LogOut, 3000);
}

function Success_RedirectPage() {
	setInterval(Page_Dashboard, 3000);
}

function Page_Dashboard() {
	window.location.href = "/Dashboard/Dashboard/";
}

function Automatic_LogOut() {
	setInterval(Page_LogOut, 900000);
}

function Page_LogOut() {
	window.location.href = "/Home/Index/";
}

function noBack() {
	window.history.forward();
}

function disablePrev() {
	window.history.forward()
}

function ClosePopUpModal() {
	$("#PopUpLogin").modal('hide');
}