
$(document).ready(function () {

    GetSessionLogin();

    FocusTabMenu();

    CountNewOrder();
    $('.date').datepicker({
        dateFormat: "dd/mm/yy"
    });

    $(document).on("wheel", "input[type=number]", function (e) {
        $(this).blur();
    });


}); //end document.ready


//
function FocusTabMenu() {

    var url = window.location.pathname;

    switch (url) {
        case "/Home/Index":
            $('#tabHome').addClass('active');
            break;
        case "/Item/Index":
            $('#tabItem').addClass('active');
            break;
        case "/ItemSale/Index":
            $('#tabItemSale').addClass('active');
            break;
        case "/Banner/Index":
            $('#tabBanner').addClass('active');
            break;
        case "/VoucherCustomer/Index":
            $("#ulVoucher").attr("aria-expanded", "true");
            $("#ulVoucher").addClass("collapse in");
            $('#tabVoucher').addClass('active');
            $('#tabVoucherCustomer').addClass('active');
            break;
        case "/VoucherIntroduce/Index":
            $("#ulVoucher").attr("aria-expanded", "true");
            $("#ulVoucher").addClass("collapse in");
            $('#tabVoucher').addClass('active');
            $('#tabVoucherIntroduce').addClass('active');
            break;
        case "/Order/Index":
            $('#tabOrder').addClass('active');
            break;
        case "/Customer/Index":
            $('#tabCustomer').addClass('active');
            break;
        case "/Config/Index":
            $('#tabConfig').addClass('active');
            break;
        case "/User/Index":
            $('#tabUser').addClass('active');
            break;
        case "/Category/Index":
            $('#tabCategory').addClass('active');
            break;
        default:
            break;
    }

}


//lấy thông tin đối tượng vừa đăng nhập
function GetSessionLogin() {
    $.ajax({
        url: '/Home/GetUserLogin',
        type: 'GET',
        success: function (response) {
            $("#userNameLogin").text(response.UserName);
            var role = response.Role;
            if (role != 1) {
                $("#tabUser").hide();
            }
            if (role != 2 && role != 1) {
                $("#tabCategory").hide();
                $("#tabItem").hide();
                $("#tabItemSale").hide();
                $("#tabBanner").hide();
                $("#tabVoucher").hide();
                $("#tabConfig").hide();
            }
            if (role != 3 && role != 1) {
                $("#tabCustomer").hide();
            }
           
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });

}

function CountNewOrder() {
    $.ajax({
        url: "/Home/CountNewOrder",
        success: function (res) {
            $('#all-new-order').text(res);
        }
    })
}
