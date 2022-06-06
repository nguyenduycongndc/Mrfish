﻿
$(document).ready(function () {
    //tự động chọn option có cùng giá trị
    var typeNews = $("#cbbType").attr("value");
    $("#cbbType option").each(function () {
        if (typeNews == $(this).val()) {
            $(this).attr('selected', 'selected');
        }
    });

    var typeSendNews = $("#cbbTypeSend").attr("value");
    $("#cbbTypeSend option").each(function () {
        if (typeSendNews == $(this).val()) {
            $(this).attr('selected', 'selected');
        }
    });

    //clear text when close modal
    $('.modal').on('hidden.bs.modal', function () {
        $(this).find("input,textarea").val('');
    });

    //change option in Combobox
    $('#status').on("change", function () {
        searchWarrantyCard();
    });

    $('#type').on('change', function () {
        searchPoint();
    });

    $('#itemStatus').on('change', function () {
        SearchItem();
    });

    //auto trim input text
    $('input[type="text"]').change(function () {
        this.value = $.trim(this.value);
    });

    $('#place').on('change', function () {
        LoadPlaceCreateShop();
    });

    //auto format number input
    $('.number').keyup(function () {
        $val = cms_decode_currency_format($(this).val());
        $(this).val(cms_encode_currency_format($val));
    });
}); //end document.ready


const SUCCESS = 1;
const ERROR = 0;
const DUPLICATE_NAME = 2;
const CAN_NOT_DELETE = 2;
const WRONG_PASSWORD = 2;
const NOT_ADMIN = 3;
const EXISTING = 2;
const FAIL_LOGIN = 2;
const URL_ADD_IMG_DEFAULT = "/Uploads/files/add_img.png";


//đăng nhập
function Login() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var phone = $("#txtUsernameLogin").val();
    var password = $("#txtPasswordLogin").val();
    if (phone == "" || password == "") {
        swal({
            title: "Vui lòng nhập đầy đủ!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Home/UserLogin',
        data: { phone: phone, password: password },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {
                window.location.assign("/Home/Index");
            } else
                if (response == FAIL_LOGIN) {
                    swal({
                        title: "Sai thông tin đăng nhập!",
                        text: "",
                        icon: "warning"
                    })
                    $("#txtUsernameLogin").val("");
                    $("#txtPasswordLogin").val("");
                } else {
                    swal({
                        title: "Hệ thống đang bảo trì",
                        text: "",
                        icon: "warning"
                    })
                }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function logout() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Home/Logout',
        data: {},
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {
                location.reload();
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

//đổi mật khẩu
function changePassword() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var currentPassword = $.trim($("#txtCurrentPassword").val());
    var newPassword = $.trim($("#txtNewPassword").val());
    var confirmPassword = $.trim($("#txtConfirmPassword").val());

    if (currentPassword == "" || newPassword == "" || confirmPassword == "") {
        swal({
            title: "Vui lòng nhập đầy đủ!",
            text: "",
            icon: "warning"
        })
        return;
    }
    if (newPassword.length < 6) {
        swal({
            title: "Mật khẩu tối thiểu 6 ký tự!",
            text: "",
            icon: "warning"
        })
        return;
    }
    if (newPassword != confirmPassword) {
        $("#txtConfirmPassword").val("");
        swal({
            title: "Mật khẩu xác nhận không đúng",
            text: "",
            icon: "warning"
        })
        return;
    }

    $.ajax({
        url: '/User/ChangePassword',
        data: {
            CurrentPassword: currentPassword,
            NewPassword: newPassword
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {
                $("#changePassword").modal("hide");
                swal({
                    title: "Đổi mật khẩu thành công",
                    text: "",
                    icon: "success"
                })
            } else
                if (response == WRONG_PASSWORD) {
                    $("#txtCurrentPassword").val("");
                    swal({
                        title: "Mật khẩu cũ không đúng",
                        text: "",
                        icon: "warning"
                    })
                } else {
                    swal({
                        title: "Không thể đổi mật khẩu",
                        text: "",
                        icon: "warning"
                    })
                }
        },
        error: function (result) {
            console.log(result.responseText);
            swal({
                title: "Có lỗi",
                text: "",
                icon: "warning"
            })
        }
    });
}

// start xóa yêu cầu đổi quà
function deleteRequest(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Request/DeleteRequest',
                    data: { RequestID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {

                            swal({
                                title: "Xóa thành công!",
                                text: "",
                                icon: "success"
                            })

                            $.ajax({
                                url: '/Request/Search',
                                data: {
                                    Page: $("#txtPageCurrent").val(),
                                    RequestCode: $("#txtRequestCodeSearch").val(),
                                    Status: $("#cbbStatus").val(),
                                    Type: $("#cbbType").val(),
                                    FromDate: $("#txtRequestFromDate").val(),
                                    ToDate: $("#txtRequestToDate").val()
                                },
                                type: 'POST',
                                success: function (response) {
                                    $("#tableRequest").html(response);
                                },
                                error: function (result) {
                                    console.log(result.responseText);
                                }
                            });

                        } else
                            if (response == CAN_NOT_DELETE) {
                                swal({
                                    title: "Không thể xóa!",
                                    text: "Yêu cầu này đã được xử lý.",
                                    icon: "warning"
                                })
                            } else {
                                swal({
                                    title: "Không thể xóa!",
                                    text: "Có lỗi xảy ra.",
                                    icon: "warning"
                                })
                            }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}// end xóa yêu cầu đổi quà

// start chấp nhận yêu cầu đổi quà
function acceptRequest(id, customerID, statusRequest) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var note = $.trim($("#noteRequest").val());
    var requestGiftName = $("#requestGiftName").html();

    if (statusRequest == 2) {

        swal({
            title: "Bạn chắc chắn hủy yêu cầu chứ?",
            text: "",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    $.ajax({
                        url: '/Request/AcceptRequest',
                        data: {
                            //status request: gửi trạng thái xem thực hiện việc xác nhận hay hủy yêu cầu
                            StatusRequest: statusRequest,
                            RequestID: id,
                            CustomerID: customerID,
                            RequestGiftName: requestGiftName,
                            Note: note
                        },
                        type: 'POST',
                        success: function (response) {
                            if (response == SUCCESS) {
                                $('#questDetail').modal('hide');
                                swal({
                                    title: "Đã hủy yêu cầu!",
                                    text: "",
                                    icon: "success"
                                })

                                $.ajax({
                                    url: '/Request/Search',
                                    data: {
                                        Page: $("#txtPageCurrent").val(),
                                        RequestCode: $("#txtRequestCodeSearch").val(),
                                        Status: $("#cbbStatus").val(),
                                        Type: $("#cbbType").val(),
                                        FromDate: $("#txtRequestFromDate").val(),
                                        ToDate: $("#txtRequestToDate").val()
                                    },
                                    type: 'POST',
                                    success: function (response) {
                                        $("#tableRequest").html(response);
                                    },
                                    error: function (result) {
                                        console.log(result.responseText);
                                    }
                                });

                            } else
                                swal({
                                    title: "Có lỗi xảy ra!",
                                    text: "",
                                    icon: "warning"
                                })
                        },
                        error: function (result) {
                            console.log(result.responseText);
                        }
                    });
                }
            })
    } else
        $.ajax({
            url: '/Request/AcceptRequest',
            data: {
                //status request: gửi trạng thái xem thực hiện việc xác nhận hay hủy yêu cầu
                StatusRequest: statusRequest,
                RequestID: id,
                CustomerID: customerID,
                RequestGiftName: requestGiftName,
                Note: note
            },
            type: 'POST',
            success: function (response) {
                if (response == SUCCESS) {
                    $('#questDetail').modal('hide');
                    swal({
                        title: "Thành công!",
                        text: "Yêu cầu đã được xác nhận",
                        icon: "success"
                    })

                    $.ajax({
                        url: '/Request/Search',
                        data: {
                            Page: $("#txtPageCurrent").val(),
                            RequestCode: $("#txtRequestCodeSearch").val(),
                            Status: $("#cbbStatus").val(),
                            Type: $("#cbbType").val(),
                            FromDate: $("#txtRequestFromDate").val(),
                            ToDate: $("#txtRequestToDate").val()
                        },
                        type: 'POST',
                        success: function (response) {
                            $("#tableRequest").html(response);
                        },
                        error: function (result) {
                            console.log(result.responseText);
                        }
                    });

                } else
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "",
                        icon: "warning"
                    })
            },
            error: function (result) {
                console.log(result.responseText);
            }
        });
}// end chấp nhận yêu cầu đổi quà

// start thông tin chi tiết 1 người dùng
function updateRole(id) {
    var role = $('input[name=radio-group]:checked').val();
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/User/UpdateRole',
        data: { ID: id, Role: role },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#modalRole').modal('hide');
                swal({
                    title: "Phân quyền thành công!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/User/Search',
                    data: {
                        Page: $("#txtPageCurrent").val(),
                        Phone: $("#txtPhoneUser").val(),
                        FromDate: $("#txtFromDateUser").val(),
                        ToDate: $("#txtToDateUser").val()
                    },
                    type: 'POST',
                    success: function (response) {
                        $("#tableUser").html(response);
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });

            } else {
                swal("Có lỗi", "", "warning");
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết 1 người dùng


function LoadBanner(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Banner/LoadBanner',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#UpdateBanner").html(response);
            $('#EditBanner').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
// start thông tin chi tiết 1 người dùng
function getUserDetail(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/User/GetUserDetail',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#divUserDetail").html(response);
            $('#EditUser').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết 1 người dùng

// start thông tin chi tiết 1 yêu cầu đổi quà
function getRequestDetail(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Request/GetRequestDetail',
        data: { RequestID: id },
        type: 'POST',
        success: function (response) {
            $("#divRequestDetail").html(response);
            $('#questDetail').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết 1 yêu cầu đổi quà

// start tìm kiếm yêu cầu đổi quà
function searchRequest() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var requestCode = $.trim($("#txtRequestCodeSearch").val());
    var status = $("#cbbStatus").val();
    var type = $("#cbbType").val();
    var fromDate = $.trim($("#txtRequestFromDate").val());
    var toDate = $.trim($("#txtRequestToDate").val());

    $.ajax({
        url: '/Request/Search',
        data: {
            Page: 1,
            RequestCode: requestCode,
            Status: status,
            Type: type,
            FromDate: fromDate,
            ToDate: toDate
        },
        type: 'POST',
        success: function (response) {
            $("#tableRequest").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end tìm kiếm yêu cầu đổi quà

// start tìm kiếm tin tức
function searchNews() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var title = $.trim($("#txtTitle").val());
    var createUser = $("#cbbCreateUser").val();
    var type = $("#cbbTypeNews").val();
    var status = $("#cbbStatusNews").val();
    var fromDate = $.trim($("#txtNewsFromDate").val());
    var toDate = $.trim($("#txtNewsToDate").val());
    $.ajax({
        url: '/News/Search',
        data: {
            Page: 1,
            Title: title,
            CreateUserID: createUser,
            Type: type,
            Status: status,
            FromDate: fromDate,
            ToDate: toDate
        },
        type: 'POST',
        success: function (response) {
            $("#tableNews").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end tìm kiếm tin tức

// start danh sách thiết lập quà hoặc voucher
function searchConfigGift() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Config/SearchConfigGift',
        data: {
            Page: 1
        },
        type: 'POST',
        success: function (response) {
            $("#tableConfigGift").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end danh sách thiết lập quà hoặc voucher

//Banner
function CreateBanner() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var title = $.trim($("#titleCreate").val());
    var typeSend = $("#slTypeSend").val();
    var type = $("#slType").val();
    var Note = $.trim(CKEDITOR.instances['NoteCreate'].getData());
    var url = $('#AddImgLogoPlace').attr('src');
    var icheck = $('#_checkMes').val();
    debugger
    if (title.length == "" || typeSend.length == "" || type.length == "" || url == "/Uploads/files/add_img.png") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    } else {
        $.ajax({
            url: '/Banner/CreateBanner',
            type: 'POST',
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify({
                title: title,
                typeSend: typeSend,
                type: type,
                content: Note,
                imageUrl: url,
                icheck: icheck,
            }),
            success: function (res) {
                
                if (res.Code == 200) {
                    swal({
                        title: "Tạo banner thành công!",
                        text: "",
                        icon: res.Status == SUCCESS ? 'success' : 'error'
                    }).then((rp) => {
                        if (rp) {
                            location.reload();
                        }
                    })
                    window.location = '/Banner/Index';
                    SearchBanner();
                } else if (res.Code == 418) {
                    swal({
                        title: "Tạo banner thất bại!",
                        text: "Banner này đã tồn tai",
                        icon: 'error',
                    })
                }
            },
            error: function (ex) {
                swal({
                    title: " Failed!",
                    text: "Thên mới thất bại!",
                    icon: "warning",
                    button: "Close",
                })
            }
        });
    }
}

function CreateVoucherIntroduce() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var codeVouCreate = $.trim($("#codeVouCreate").val());
    var nameVouCreate = $.trim($("#nameVouCreate").val());
    var typeDiscountVouCreate = $("#slTypeDiscountVouCreate").val();
    var discountVouCreate = $("#discountVouCreate").val();
    var Note = $.trim(CKEDITOR.instances['NoteVouCreate'].getData());
    var url = $('#AddImgLogoPlace').attr('src');
    debugger
    if (codeVouCreate.length == "" || nameVouCreate.length == "" || discountVouCreate.length == "" || typeDiscountVouCreate.length == "" || url == "/Uploads/files/add_img.png") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    } else {
        $.ajax({
            url: '/VoucherIntroduce/CreateVoucherIntroduce',
            type: 'POST',
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify({
                codeVouCreate: codeVouCreate,
                nameVouCreate: nameVouCreate,
                typeDiscountVouCreate: typeDiscountVouCreate,
                discountVouCreate: discountVouCreate,
                AddLogoVoucherCus: url,
                Note: Note,
            }),
            success: function (res) {
                if (res.Code == 200) {
                    swal({
                        title: "Tạo mới thành công!",
                        text: "",
                        icon: "success",/* : 'error'*/
                    }).then((rp) => {
                        if (rp) {
                            location.reload();
                        }
                    })
                    window.location = '/VoucherIntroduce/Index';
                    SearchBanner();
                } else if (res.Code == 423) {
                    swal({
                        title: "Tạo mới thất bại!",
                        text: "Mức giảm không được lớn hơn 100%",
                        icon: "error",
                    })
                }

            },
            error: function (ex) {
                swal({
                    title: " Failed!",
                    text: "Thên mới thất bại!",
                    icon: "warning",
                    button: "Close",
                })
            }
        });
    }
}

function CreateVoucherCustomer() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var codeVoucherCustomerCreate = $.trim($("#codeVoucherCustomerCreate").val());
    var nameVoucherCustomerCreate = $.trim($("#nameVoucherCustomerCreate").val());
    var quantityVoucherCustomerCreate = $("#quantityVoucherCustomerCreate").val();
    var slTypeDiscountCreate = $("#slTypeDiscountCreate").val();
    var discountCreate = $("#discountCreate").val();
    var slStatusVoucherCustomerCreate = 1 /*$("#slStatusVoucherCustomerCreate").val()*/;
    var AddLogoVoucherCus = $('#AddImgLogoPlace').attr('src');
    var testRank = $("#testRank").val();
    var fromDateVoucherCreate = $("#fromDateVoucherCreate").val();
    var toDateVoucherCreate = $("#toDateVoucherCreate").val();
    var NoteCreateVoucherCustomer = $.trim(CKEDITOR.instances['NoteCreateVoucherCustomer'].getData());
    debugger
    if (codeVoucherCustomerCreate.length == ""
        || nameVoucherCustomerCreate.length == ""
        || quantityVoucherCustomerCreate.length == ""
        || slTypeDiscountCreate.length == ""
        || discountCreate.length == ""
        || slStatusVoucherCustomerCreate.length == ""
        || testRank.length == ""
        || fromDateVoucherCreate.length == ""
        || toDateVoucherCreate.length == ""
        || AddLogoVoucherCus == "/Uploads/files/add_img.png") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    } else if(codeVoucherCustomerCreate.length < 6) {
        swal({
            title: "Thông báo",
            text: "Mã Voucher ít nhất 6 ký tự!",
            icon: "warning"
        })
        return;
    } else if(nameVoucherCustomerCreate.length < 6) {
        swal({
            title: "Thông báo",
            text: "Tên Voucher ít nhất 6 ký tự!",
            icon: "warning"
        })
        return;
    } else {
        $.ajax({
            url: '/VoucherCustomer/CreateVoucherCustomer',
            type: 'POST',
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify({
                codeVoucherCustomerCreate: codeVoucherCustomerCreate,
                nameVoucherCustomerCreate: nameVoucherCustomerCreate,
                quantityVoucherCustomerCreate: quantityVoucherCustomerCreate,
                slTypeDiscountCreate: slTypeDiscountCreate,
                discountCreate: discountCreate,
                slStatusVoucherCustomerCreate: slStatusVoucherCustomerCreate,
                testRank: testRank,
                AddLogoVoucherCus: AddLogoVoucherCus,
                fromDateVoucherCreate: fromDateVoucherCreate,
                toDateVoucherCreate: toDateVoucherCreate,
                NoteCreateVoucherCustomer: NoteCreateVoucherCustomer,
            }),
            success: function (res) {
                if (res.Code == 200) {
                    swal({
                        title: "Tạo voucher thành công!",
                        text: "",
                        icon: 'success',
                    }).then((rp) => {
                        if (rp) {
                            location.reload();
                        }
                    })
                    window.location = '/VoucherCustomer/Index';
                    SearchVoucherCustomer();
                } else if (res.Code == 413) {
                    swal({
                        title: "Tạo voucher thất bại!",
                        text: "Thời gian không hợp lệ",
                        icon: 'error',
                    })
                } else if (res.Code == 418) {
                    swal({
                        title: "Tạo voucher thất bại!",
                        text: "Mã Voucher này đã tồn tại",
                        icon: 'error',
                    })
                } else if (res.Code == 423) {
                    swal({
                        title: "Tạo voucher thất bại!",
                        text: "Mức giảm không được lớn hơn 100%",
                        icon: 'error',
                    })
                }else if (res.Code == 500) {
                    swal({
                        title: "Tạo voucher thất bại!",
                        text: "Vui lòng kiểm tra lại dữ liệu đầu vào",
                        icon: 'error',
                    })
                }
            },
            error: function (ex) {
                swal({
                    title: " Failed!",
                    text: "Thên mới thất bại!",
                    icon: "warning",
                    button: "Close",
                })
            }
        });
    }
}

// start tạo user
function createUser() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var userName = $.trim($("#txtNameCreateUser").val());
    var phone = $.trim($("#txtPhoneCreateUser").val());
    var dob = $("#txtDOB").val();
    var address = $.trim($("#txtAddress").val());
    var sex = $("#slSex").val();
    var email = $.trim($("#mail").val());
    var role = $("#slRole").val();
    //var status = $("#slStatus").val();
    var password = $.trim($("#txtPasswordCreateUser").val());
    var confirmpass = $.trim($("#txtconfirmPasswordCreateUser").val());
    var note = $.trim($("#txtNote").val());
    var url = $('#AddImgLogoPlace').attr('src');
    if (password != confirmpass) {
        swal({
            title: "Thông báo",
            text: "Mật khẩu và xác nhận mật khẩu khác nhau",
            icon: "warning"
        })
        return;
    }
    debugger
    if (userName.length == "" || phone.length == "" || dob.length == ""
        || role.length == "" || password.length == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    } else
        if (!isNumeric(phone)) {
            swal({
                title: "Thông báo",
                text: "Số điện thoại chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        }
        else if (phone.length != 10 || phone[0] != 0) {
            swal({
                title: "Thông báo",
                text: "Số điện thoại không hợp lệ!",
                icon: "warning"
            })
            return;
        }
        else if (password.length < 6) {
            swal({
                title: "Thông báo",
                text: "Mật khẩu có ít nhất 6 ký tự!",
                icon: "warning"
            })
            return;
        }
        else {
            $.ajax({
                url: '/User/CreateUser',
                type: 'POST',
                dataType: "json",
                contentType: 'application/json',
                data: JSON.stringify({
                    UserName: userName,
                    Phone: phone,
                    DOB: dob,
                    Address: address,
                    Sex: sex,
                    Email: email,
                    Role: role,
                    //Status: status,
                    password: password,
                    Note: note,
                    AvatarUrl: url,
                }),
                success: function (res) {
                    swal({
                        title: "Tạo tài khoản thành công!",
                        text: "",
                        icon: res.Status == SUCCESS ? 'success' : 'error'
                    }).then((rp) => {
                        if (rp) {
                            location.reload();
                        }
                    })
                    window.location = '/User/Index';
                    searchUser();
                },
                error: function (ex) {
                    swal({
                        title: " Failed!",
                        text: "Thên mới thất bại!",
                        icon: "warning",
                        button: "Close",
                    })
                }
                //success: function (response) {
                //    debugger;
                //    if (response == SUCCESS) {

                //        $('#createUser').modal('hide');
                //        swal({
                //            title: "Tạo tài khoản thành công!",
                //            text: "",
                //            icon: "success"
                //        })
                //        window.location = '/User/Index';
                //        searchUser();
                //    } else
                //        if (response == EXISTING) {
                //            swal({
                //                title: "Không thể tạo tài khoản!",
                //                text: "Số điện thoại đã tồn tại. Vui lòng sử dụng số điện thoại khác.",
                //                icon: "warning"
                //            })
                //            $("#createUser #txtPhoneCreateUser").val("");
                //        }
                //        else if (response == NOT_ADMIN) {
                //            $('#createUser').modal('hide');
                //            swal({
                //                title: "Bạn không có quyền tạo tài khoản.",
                //                text: "",
                //                icon: "warning"
                //            })
                //        } else {
                //            swal({
                //                title: "Có lỗi xảy ra!",
                //                text: "",
                //                icon: "warning"
                //            })
                //        }

                //},
                //error: function (result) {
                //    console.log(result.responseText);
                //}
            });
        }
}// end tạo user

function addUser() {
    window.location = "/User/addUser";
}
// start tìm kiếm user
function searchUser() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var phone = $.trim($("#txtPhoneUser").val());
    var _role = $("#slRole").val();
    var _fromDate = $('#txtFromDate').val().trim();
    var _toDate = $('#txtToDate').val().trim();
    var _status = $('#slStatus').val();
    $.ajax({
        url: '/User/Search',
        data: {
            Page: 1,
            Phone: phone,
            Role: _role,
            FromDate: _fromDate,
            ToDate: _toDate,
            Status: _status,
        },
        type: 'POST',
        success: function (response) {
            $("#tableUser").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });

}// end tìm kiếm user

// start tìm kiếm lô hàng
function searchBatch() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }

    var batchCode = $.trim($("#txtBatchSearch").val());
    var fromDate = $.trim($("#txtBatchFromDate").val());
    var toDate = $.trim($("#txtBatchToDate").val());
    $.ajax({
        url: '/Batch/Search',
        data: {
            Page: 1,
            BatchCode: batchCode,
            FromDate: fromDate,
            ToDate: toDate,
        },
        type: 'POST',
        success: function (response) {
            $("#tableBatch").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end tìm kiếm lô hàng

function isSpace(string) {
    if (/\s/.test(string)) {
        return true;
    }
}

function isNumeric(s) {
    var re = new RegExp("^[0-9,]+$");
    return re.test(s);
}

// start tạo lô hàng mới
function createBatch() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var code = $.trim($("#createBatch #code").val());
    var name = $.trim($("#createBatch #name").val());
    var price = $.trim($("#createBatch #price").val());
    var qty = cms_decode_currency_format($.trim($("#createBatch #qty").val()));
    var point = $.trim($("#createBatch #point").val());
    var note = $.trim($("#createBatch #note").val());
    var item = $.trim($("#createBatch #showProduct").val());
    //var prdName = $.trim($("#createBatch #txtProductName").val());


    if (code.length == "" || name.length == "" || price.length == "" || qty.length == "" || point.length == "") {
        swal({
            title: "Thông báo",
            text: "Chỉ được phép bỏ trống phần ghi chú!",
            icon: "warning"
        })
        return;
    } else
        if (item == null || item == "") {
            swal({
                title: "Thông báo",
                text: "Vui lòng chọn sản phẩm trước khi lưu",
                icon: "warning"
            })
            return;
        } else
            if (!isNumeric(price)) {
                swal({
                    title: "Thông báo",
                    text: "Giá tiền chỉ được phép nhập số!",
                    icon: "warning"
                })
                return;
            } else
                if (!isNumeric(qty)) {
                    swal({
                        title: "Thông báo",
                        text: "Số lượng chỉ được phép nhập số!",
                        icon: "warning"
                    })
                    return;
                } else
                    if (!isNumeric(point)) {
                        swal({
                            title: "Thông báo",
                            text: "Số điểm chỉ được phép nhập số!",
                            icon: "warning"
                        })
                        return;
                    } else
                        if (qty <= 0) {
                            swal({
                                title: "Thông báo",
                                text: "Số lượng phải lớn hơn 0",
                                icon: "warning"
                            })
                            return;
                        } else
                            if (isSpace(code)) {
                                swal({
                                    title: "Thông báo",
                                    text: "Mã lô không được có khoảng trắng!",
                                    icon: "warning"
                                })
                                return;
                            } else

                                $.ajax({
                                    url: '/Batch/CreateBatch',
                                    data: $("#form_create_batch").serialize(),
                                    type: 'POST',
                                    success: function (response) {
                                        if (response == SUCCESS) {
                                            $('#createBatch').modal('hide');
                                            swal({
                                                title: "Thành công!",
                                                text: "",
                                                icon: "success"
                                            })
                                            window.location = "/Batch/Index";

                                            //$.ajax({
                                            //    url: '/Batch/Search',
                                            //    data: {
                                            //        Page: $("#txtPageCurrent").val(),
                                            //        BatchCode: $("#txtBatchSearch").val(),
                                            //        FromDate: $("#txtBatchFromDate").val(),
                                            //        ToDate: $("#txtBatchToDate").val()
                                            //    },
                                            //    type: 'POST',
                                            //    success: function (response) {
                                            //        $("#tableBatch").html(response);
                                            //    },
                                            //    error: function (result, status, err) {
                                            //        console.log(result.responseText);
                                            //        console.log(status.responseText);
                                            //        console.log(err.Message);
                                            //    }
                                            //});
                                            searchBatch();

                                        } else
                                            if (response == DUPLICATE_NAME) {
                                                swal({
                                                    title: "Không thể tạo lô hàng!",
                                                    text: "Mã lô hàng đã tồn tại. Vui lòng sử dụng mã khác.",
                                                    icon: "warning"
                                                })
                                                $("#createBatch #code").val("");
                                            } else {
                                                swal({
                                                    title: "Có lỗi xảy ra!",
                                                    text: "Không thể tạo lô hàng.",
                                                    icon: "warning"
                                                })
                                            }
                                    },
                                    error: function (result) {
                                        console.log(result.responseText);
                                    }
                                });
}// end tạo lô hàng mới


// start xóa user
function deleteUser(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/User/DeleteUser',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Xóa thành công!",
                                text: "",
                                icon: "success"
                            })

                            $.ajax({
                                url: '/User/Search',
                                data: {
                                    Page: $("#txtPageCurrent").val(),
                                    Phone: $("#txtPhoneUser").val(),
                                    FromDate: $("#txtFromDateUser").val(),
                                    ToDate: $("#txtToDateUser").val()
                                },
                                type: 'POST',
                                success: function (response) {
                                    $("#tableUser").html(response);
                                },
                                error: function (result) {
                                    console.log(result.responseText);
                                }
                            });

                        } else {
                            swal({
                                title: "Có lỗi xảy ra!",
                                text: "",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}// end xóa user

// start xóa lô hàng
function deleteBatch(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Batch/DeleteBatch',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Xóa thành công!",
                                text: "",
                                icon: "success"
                            })

                            $.ajax({
                                url: '/Batch/Search',
                                data: {
                                    Page: $("#txtPageCurrent").val(),
                                    BatchCode: $("#txtBatchSearch").val(),
                                    FromDate: $("#txtBatchFromDate").val(),
                                    ToDate: $("#txtBatchToDate").val()
                                },
                                type: 'POST',
                                success: function (response) {
                                    $("#tableBatch").html(response);
                                },
                                error: function (result) {
                                    console.log(result.responseText);
                                }
                            });

                            window.location = "/Batch/Index";

                        } else
                            if (response == CAN_NOT_DELETE) {
                                swal({
                                    title: "Không thể xóa!",
                                    text: "Trong lô hàng đã có sản phẩm được dùng.",
                                    icon: "warning"
                                })
                            }
                            else
                                if (response == 96) {
                                    swal({
                                        title: "Mất mạng!",
                                        text: "Kiểm tra kết nối internet.",
                                        icon: "warning"
                                    })
                                }
                                else {
                                    swal({
                                        title: "Có lỗi xảy ra!",
                                        text: "",
                                        icon: "warning"
                                    })
                                }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}// end xóa lô hàng

// start xóa thiết lập đổi điểm với quà, voucher
function deleteConfigCard(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Config/DeleteConfigCard',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Xóa thành công!",
                                text: "",
                                icon: "success"
                            })

                            $.ajax({
                                url: '/Config/SearchConfigCard',
                                data: {
                                    Page: $("#txtPageCurrentCard").val()
                                },
                                success: function (response) {
                                    $("#tableConfigCard").html(response);
                                }
                            });

                            //searchConfigGift();

                        } else {
                            swal({
                                title: "Không thể xóa!",
                                text: "Có lỗi.",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })
}// end xóa thiết lập đổi điểm với thẻ cào


// start xóa thiết lập đổi điểm với quà, voucher
function deleteConfigGift(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Config/DeleteConfigGift',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Xóa thành công!",
                                text: "",
                                icon: "success"
                            })

                            //$.ajax({
                            //    url: '/Config/SearchConfigGift',
                            //    data: {
                            //        Page: $("#txtPageCurrent").val()
                            //    },
                            //    type: 'POST',
                            //    success: function (response) {
                            //        $("#tableConfigGift").html(response);
                            //    }
                            //});

                            searchConfigGift();

                        } else {
                            swal({
                                title: "Không thể xóa!",
                                text: "Có lỗi.",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })
}// end xóa thiết lập đổi điểm với quà, voucher

// start xóa bài viết
function deleteNews(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/News/DeleteNews',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Xóa thành công!",
                                text: "",
                                icon: "success"
                            })
                            window.location = "/News/Index";

                            $.ajax({
                                url: '/News/Search',
                                data: {
                                    Page: $("#txtPageCurrent").val(),
                                    Title: $("#txtTitle").val(),
                                    CreateUserID: $("#cbbCreateUser").val(),
                                    Type: $("#cbbTypeNews").val(),
                                    Status: $("#cbbStatusNews").val(),
                                    FromDate: $("#txtNewsFromDate").val(),
                                    ToDate: $("#txtNewsToDate").val()
                                },
                                type: 'POST',
                                success: function (response) {
                                    $("#tableNews").html(response);
                                }
                            });

                        } else {
                            swal({
                                title: "Không thể xóa!",
                                text: "Có lỗi.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}// end xóa bài iết



// start thông tin chi tiết bài viết
function getNewsDetail(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    window.location = "UpdateNews";
    //window.open('UpdateNews');
    $.ajax({
        url: '/News/GetNewsDetail',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#divBatchDetail").html(response);
            window.location.href = "_UpdateNews.html";
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết bài viết

// start thông tin chi tiết 1 lô hàng
function getBatchDetail(batchID) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $('#modalLoad').modal("show");
    $.ajax({
        url: '/Batch/GetBatchDetail',
        data: { BatchID: batchID },
        type: 'POST',
        success: function (response) {
            $("#divBatchDetail").html(response);
            $('#mdBatchDetail').modal('show');
            $('#modalLoad').modal("hide");
        },
        error: function (result) {
            $('#modalLoad').modal("hide");
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết 1 lô hàng

// start in mã QR của lô hàng
function printQrBatch() {
    swal({
        title: "Thông báo",
        text: "Chức năng đang xây dựng!",
        icon: "warning"
    })
}// end in mã QR của lô hàng


//sửa thiết lập điểm vs thẻ cào
function updateConfigCard(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var point = cms_decode_currency_format($.trim($('#updateConfigCard #txtPoint').val()));
    var description = $.trim($('#updateConfigCard #txtDescription').val());

    if (point == "" || description == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(point)) {
        swal({
            title: "Thông báo",
            text: "Số điểm chỉ được phép nhập số.",
            icon: "warning"
        })
        return;
    }
    if (point < 0) {
        swal({
            title: "Thông báo",
            text: "Số điểm không được nhỏ hơn 0.",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/UpdateConfigCard",
        data: {
            ID: id,
            Point: point,
            Description: description
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#updateConfigCard').modal('hide');
                swal({
                    title: "Thành công!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/Config/SearchConfigCard',
                    data: {
                        Page: $("#txtPageCurrentCard").val()
                    },
                    success: function (response) {
                        $("#tableConfigCard").html(response);
                    }
                });

            } else {
                swal({
                    title: "Lỗi",
                    text: "Không thể chỉnh sửa thiết lập.",
                    icon: "warning"
                })
            }
        },
        error: function () {
            swal({
                title: "Lỗi hệ thống",
                text: "",
                icon: "warning"
            })
        }
    });

}
//sửa thiết lập điểm vs thẻ cào


//thiết lập điểm vs thẻ cào
function createConfigCard(type) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var price = $.trim($('#cbbPriceConfigCard').val());
    var point = cms_decode_currency_format($('#txtPointConfigCard').val().trim());
    var description = $.trim($('#txtDescriptionConfigCard').val());
    var telecomType = $.trim($('#cbbTelecomTypeConfigCard').val());

    if (price == "" || point == "" || telecomType == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(point)) {
        swal({
            title: "Số điểm chỉ được phép nhập số.",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/CreateConfigCard",
        data: {
            Price: price,
            Point: point,
            Description: description,
            Type: type,
            TelecomType: telecomType
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#createConfigCard').modal('hide');
                swal({
                    title: "Thành công!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/Config/SearchConfigCard',
                    data: {
                        Page: $("#txtPageCurrentCard").val()
                    },
                    success: function (response) {
                        $("#tableConfigCard").html(response);
                    }
                });

            } else
                if (response == EXISTING) {
                    $('#createConfigCard').modal('hide');
                    swal({
                        title: "Thông báo",
                        text: "Thẻ cào đã được thiết lập trước đó.",
                        icon: "warning"
                    })
                } else {
                    swal({
                        title: "Lỗi",
                        text: "Không thể tạo thiết lập.",
                        icon: "warning"
                    })
                }
        },
        error: function () {
            swal({
                title: "Có lỗi xảy ra.",
                text: "",
                icon: "warning"
            })
        }
    });

}
//thiết lập điểm vs thẻ cào

//sửa thiết lập điểm vs quà, voucher
function updateConfigGift(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var test = id;
    var type = $("#configGiftDetail #cbbType").val();
    var name = $.trim($('#configGiftDetail #txtName').val());
    var price = cms_decode_currency_format($.trim($('#configGiftDetail #txtPrice').val()));
    var point = cms_decode_currency_format($.trim($('#configGiftDetail #txtPoint').val()));
    var fromDate = $.trim($('#configGiftDetail #txtFromDateEdit').val());
    var toDate = $.trim($('#configGiftDetail #txtToDateEdit').val());
    var description = $.trim($('#configGiftDetail #txtDescription').val());
    var url = $("#configGiftDetail #tagImg2").attr("src");
    var status = $('#configGiftDetail #slStatus').val();

    if (name == "" || price == "" || point == "" || fromDate == "" || toDate == "" || description == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    if (url == URL_ADD_IMG_DEFAULT) {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn ảnh cho thiết lập!",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(price)) {
        swal({
            title: "Giá tiền chỉ được phép nhập số.",
            text: "",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(point)) {
        swal({
            title: "Số điểm chỉ được phép nhập số.",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/UpdateConfigGift",
        data: {
            ID: test,
            Name: name,
            Price: price,
            Point: point,
            UrlImage: url,
            Description: description,
            Type: type,
            FromDate: fromDate,
            ToDate: toDate,
            Status: status
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#configGiftDetail').modal('hide');
                swal({
                    title: "Thành công!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/Config/SearchConfigGift',
                    data: {
                        Page: $("#txtPageCurrent").val()
                    },
                    success: function (response) {
                        $("#tableConfigGift").html(response);
                    }
                });

            } else {
                swal({
                    title: "Không thể sửa thiết lập.",
                    text: "",
                    icon: "warning"
                })
            }
        },
        error: function () {
            swal({
                title: "Có lỗi xảy ra.",
                text: "",
                icon: "warning"
            })
        }

    });

}
//sửa thiết lập điểm vs quà, voucher


//thiết lập điểm vs quà, voucher
function createConfigGift() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var type = $("#cbbType").val();
    var name = $.trim($('#txtName').val());
    var price = cms_decode_currency_format($.trim($('#txtPrice').val()));
    var point = cms_decode_currency_format($.trim($('#txtPoint').val()));
    var fromDate = $.trim($('#txtFromDate').val());
    var toDate = $.trim($('#txtToDate').val());
    var description = $.trim($('#txtDescription').val());
    var url = $("#tagImg").attr("src");

    if (name == "" || price == "" || point == "" || fromDate == "" || toDate == "" || description == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    if (url == URL_ADD_IMG_DEFAULT) {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn ảnh cho thiết lập!",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(price)) {
        swal({
            title: "Giá tiền chỉ được phép nhập số.",
            text: "",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(point)) {
        swal({
            title: "Số điểm chỉ được phép nhập số.",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/CreateConfigGift",
        data: {
            Name: name,
            Price: price,
            Point: point,
            UrlImage: url,
            Description: description,
            Type: type,
            FromDate: fromDate,
            ToDate: toDate
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#createConfigGift').modal('hide');
                swal({
                    title: "Thành công!",
                    text: "",
                    icon: "success"
                })
                window.location = "/Config/Index";

                $.ajax({
                    url: '/Config/SearchConfigGift',
                    data: {
                        Page: $("#txtPageCurrent").val()
                    },
                    success: function (response) {
                        $("#tableConfigGift").html(response);
                    }
                });

            } else {
                swal({
                    title: "Không thể tạo thiết lập.",
                    text: "",
                    icon: "warning"
                })
            }
        },
        error: function (response) {
            console.log(response);
            swal({
                title: "Có lỗi xảy ra!",
                text: "",
                icon: "warning"
            })
        }
    });
}
//thiết lập điểm vs quà, voucher


//tạo bài viết mới
function createNews() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    //lấy nội dung html trong CKEDITOR
    var content = "a";
    var description = "a";
    var title = "Banner"
    var type = $('#cbbType').val();
    var typeSend = $('#cbbTypeSend').val();
    var status = $('#cbbStatus').val();
    if (type != 6) {
        description = $.trim($('#txt-description').val());
        content = $.trim(CKEDITOR.instances['txt-content'].getData());
        title = $.trim($('#txtTitle').val());
    }
    //var display = cms_decode_currency_format($('#txtDisplay').val());
    var url = $('#AddImgLogoPlace').attr('src');
    var sendNotify;
    if ($('#sendNotify').is(':checked', false)) {
        sendNotify = 1;
    } else {
        sendNotify = 0;
    }
    var timePush = $('#date-picker').val();
    var itemID = $('#val-item-id').val();

    if (status == "" || content == "" || title == "" || type == "" || typeSend == "" || description == "" || type == 0) {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }

    if (url == URL_ADD_IMG_DEFAULT) {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn ảnh nổi bật cho bài viết!",
            icon: "warning"
        })
        return;
    }

    $.ajax({
        url: '/News/CreateNewsDekko',
        data: {
            Content: content,
            Description: description,
            Title: title,
            Type: type,
            TypeSend: typeSend,
            UrlImage: url,
            Status: status,
            //Display: display,
            SendNotify: sendNotify,
            timePush: timePush,
            itemID: itemID
        },
        beforeSend: function () {
            if (typeSend != 4)
                $('#modalLoadSendNotify').modal("show");
            else
                $('#modalLoad').modal("show");
        },
        type: 'POST',
        success: function (response) {
            if (typeSend != 4)
                $('#modalLoadSendNotify').modal("hide");
            else
                $('#modalLoad').modal("hide");
            if (response == SUCCESS) {
                swal({
                    title: "Thành công",
                    text: "Thêm bài viết thành công",
                    icon: "success"
                })
                window.location = "/News/Index";
                setTimeout(
                    function () {
                        window.location.replace('/News/Index');
                    }, 1000);
            } else if (response == -1) {
                swal({
                    title: "Lỗi",
                    text: "Thời gian đăng bài phải là ngày giờ trong tương lai",
                    icon: "warning"
                })
            }
            else {
                swal({
                    title: "Lỗi",
                    text: "Không thể tạo bài viết mới.",
                    icon: "warning"
                })
                window.location = "/News/CreateNews";
                setTimeout(
                    function () {
                        window.location.replace('/News/CreateNews');
                    }, 1000);
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });

}
//tạo bài viết mới

//chỉnh sửa bài viết
function updateNews(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var typeSend = $('#cbbTypeSend').val();
    //lấy nội dung html trong CKEDITOR
    var type = $('#cbbType').val();
    var status = $('#cbbStatus').val();
    //var status = $('#cbbStatus').val();
    var title = $.trim($('#txtTitle').val());
    var description = $.trim($('#txt-description').val());
    var url = $('#AddImgLogoPlace').attr('src');
    var content = $.trim(CKEDITOR.instances['txt-content'].getData());
    var timePush = $('#date-picker').val();
    var itemID = $('#val-item-id').val();
    //-------------------------------------

    //var item = $('#cbbItemNews').val();


    if (content == "" || title == "" || type == "" || url == "" || description == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }

    //if (item == null && type != 4) {
    //    item = 0;
    //}

    $.ajax({
        url: '/News/UpdateNewsDekko',
        data: {
            ID: id,
            Content: content,
            Title: title,
            Type: type,
            UrlImage: url,
            Description: description,
            Status: status,
            TypeSend: typeSend,
            timePush: timePush,
            itemID: itemID
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {
                swal({
                    title: "Thành công",
                    text: "Chỉnh sửa bài viết thành công",
                    icon: "success"
                })
                window.location = "/News/Index";
                setTimeout(
                    function () {
                        window.location.replace('/News/Index');
                    }, 1000);
            } else if (response == -1) {
                swal({
                    title: "Lỗi",
                    text: "Thời gian đăng bài phải là ngày giờ trong tương lai",
                    icon: "warning"
                })
            } else {
                swal({
                    title: "Lỗi",
                    text: "",
                    icon: "warning"
                })
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });

}
//chỉnh sửa bài viết

//Tìm kiếm Card
function searchCard() {
    var seri = $('#txtSeri').val().trim();
    var fromDate = $('#txtFromDate').val().trim();
    var toDate = $('#txtToDate').val().trim();
    var status = $('#cmbStatus').val().trim();
    $.ajax({
        url: "/Card/Search",
        data: {
            Page: 1,
            Seri: seri,
            FromDate: fromDate,
            ToDate: toDate,
            Status: status
        },
        success: function (result) {
            $("#tableCard").html(result)
        },
        error: function (result) {
            console.log(result.responseText);
        }
    })
}
// thêm mới card 
function createCard() {

    $("#frmCreateCard").validate({
        ignore: ".date",
        rules: {
            CardCode: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 15
            },
            SeriNumber: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 15
            }
        },
        messages: {
            CardCode: {
                required: "Vui lòng không để trống",
                digits: "Vui lòng nhập số nguyên dương",
                minlength: "Mã thẻ phải có ít nhất 10 chứ số",
                maxlength: "Mã thẻ tối đa 15 chữ số"
            },
            SeriNumber: {
                required: "Vui lòng không để trống",
                digits: "Vui lòng nhập số nguyên dương",
                minlength: "Seri phải có ít nhất 10 chứ số",
                maxlength: "Seri tối đa 15 chữ số"
            }
        },
        submitHandler: function () {
            if ($("#dtpStartDate").val() == '' || $("#dtpExprireDate").val() == '') {
                swal({
                    title: "Thông báo",
                    text: "Vui lòng nhập đầy đủ thông tin",
                    icon: "warning"
                });
                return;
            }
            $.ajax({
                url: "/Card/addCard",
                data: $('#frmCreateCard').serialize(),
                type: 'POST',
                success: function (result) {
                    if (result == 1) {
                        swal({
                            title: "Thông báo",
                            text: "Tạo mới thành công",
                            icon: "success"
                        });
                        $('#createCard').modal("hide");
                        searchCard();
                    }
                    else if (result == DUPLICATE_NAME) {
                        swal({
                            title: "Thông báo",
                            text: "Mã thẻ hoặc seri đã tồn tại, vui lòng kiểm tra lại !",
                            icon: "warning"
                        });
                        $('#txtCardCode').val("");
                        $('#txtSeriNumber').val("");
                    }
                    else if (result == -9) {
                        swal({
                            title: "Thông báo",
                            text: "mã thẻ và seri không được trùng nhau",
                            icon: "warning"
                        });
                        $('#txtCardCode').val("");
                        $('#txtSeriNumber').val("");
                    }
                    else if (result == 3) {
                        swal({
                            title: "Kiểm Tra Lại Ngày Hết Hạn",
                            text: "Ngày hết hạn phải lớn hơn ngày bắt đầu",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Có lỗi trong quá trình thêm mới",
                            text: "Vui lòng liên hệ với bộ phận hỗ trợ khách hàng",
                            icon: "warning"
                        });
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
    });
}

// chỉnh sửa Card
function editCard(ID, Status) {
    if (Status == 1) {
        swal({
            title: "Thông Báo",
            text: "Card đã đổi, bạn không thể sửa",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Card/showEditCard",
        data: { CardID: ID },
        success: function (result) {
            $('#frmEdit').html(result);
            $('#showEdit').modal("show");
        }
    })
}
// lưu thay đổi chỉnh sửa
function SaveEdit(id) {
    $("#frmShowEdit").validate({
        ignore: ".date",
        rules: {
            CardCode: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 15
            },
            SeriNumber: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 15
            }
        },
        messages: {
            CardCode: {
                required: "Vui lòng không để trống",
                digits: "Vui lòng nhập số nguyên dương",
                minlength: "Mã thẻ phải có ít nhất 10 chứ số",
                maxlength: "Mã thẻ tối đa 15 chứ số"
            },
            SeriNumber: {
                required: "Vui lòng không để trống",
                digits: "Vui lòng nhập số nguyên dương",
                minlength: "Seri phải có ít nhất 10 chứ số",
                maxlength: "Seri tối đa 15 chữ số"
            }
        },
        submitHandler: function () {
            if ($("#dtpStartDateEdit").val() == '' || $("#dtpExprireDateEdit").val() == '') {
                swal({
                    title: "Thông báo",
                    text: "Vui lòng nhập đầy đủ thông tin",
                    icon: "warning"
                });
                return;
            }
            $.ajax({
                url: "/Card/addCard",
                data: $('#frmShowEdit').serialize(),
                success: function (result) {
                    if (result == SUCCESS) {
                        swal({
                            title: "thông báo",
                            text: "Cập nhật thành công",
                            icon: "success"
                        });
                        $('#showEdit').modal("hide");
                        searchCard();
                    }
                    else if (result == DUPLICATE_NAME) {
                        swal({
                            title: "thông báo",
                            text: "Mã thẻ hoặc seri đã tồn tại, vui lòng kiểm tra lại !",
                            icon: "warning"
                        });
                        $('#txtCardCode').val("");
                        $('#txtSeriNumber').val("");
                    }
                    else if (result == -9) {
                        swal({
                            title: "thông báo",
                            text: "mã thẻ và seri không được trùng nhau",
                            icon: "warning"
                        });
                        $('#txtCardCode').val("");
                        $('#txtSeriNumber').val("");
                    }
                    else if (result == 3) {
                        swal({
                            title: "Kiểm Tra Lại Ngày Hết Hạn",
                            text: "Ngày hết hạn phải lớn hơn ngày bắt đầu",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Lỗi rồi đại vương ơi",
                            text: "Vui lòng liên hệ với bộ phận chăm sóc khách hàng",
                            icon: "warning"
                        })
                    }
                }
            });
        }
    });
}
// xóa card
function deleteCard(id) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Card/DeleteCard',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Xóa thành công!",
                                text: "",
                                icon: "success"
                            })
                            searchCard();
                        } else {
                            swal({
                                title: "Không thể xóa!",
                                text: "Có lỗi.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}
// load District

function loadListDistrictShop(prvID) {
    $.ajax({
        url: "/Shop/LoadDistrictShop",
        data: { ProvinceID: prvID },
        success: function (result) {
            $('#ListDistrictShop').html(result);
        }
    });
}

function loadListDistrictShopCreate(prvID) {
    $.ajax({
        url: "/Shop/LoadDistrictShopCreate",
        data: { ProvinceID: prvID },
        success: function (result) {
            $('#ListDistrictShopCreate').html(result);
        }
    });
}

function loadListDistrictShopUpdate(prvID, id) {
    $.ajax({
        url: "/Shop/LoadDistrictShopUpdate",
        data: {
            ProvinceID: prvID,
            ID: id
        },
        success: function (result) {
            $('#ListDistrictShopUpdate').html(result);
        }
    });
}

function loadListDistrict(prvID) {
    $.ajax({
        url: "/Customer/LoadDistrict",
        data: { ProvinceID: prvID },
        success: function (result) {
            $('#ListDistrict').html(result);
        }
    });
}

//Tìm kiếm khách hàng
function searchCustomer() {
    var fromDate = $('#dtFromdateIndex').val();
    var toDate = $('#dtTodateIndex').val();
    var phone = $('#txtPhone').val().trim();
    var province = $('#slProvince').val();
    var rank = $('#cmbRankCus').val();
    var status = $('#slStatus').val();
    $.ajax({
        url: "/Customer/Search",
        data: {
            Page: 1,
            FromDate: fromDate,
            ToDate: toDate,
            City: province,
            Phone: phone,
            Rank: rank,
            Status: status,
        },
        success: function (result) {
            $('#ListCustomer').html(result);
        }
    });
}

function showAddPointWithChecked(inputCheck) {
    var row = $(inputCheck).parents('tr');
    var check = $(row).find('#txtchecked').prop('checked');
    var phone = $(row).find('#colPhone').html();
    if (check) {
        $('#mdAddPoint #txtPhoneNumber').val(phone);
    }
    else {
        $('#mdAddPoint #txtPhoneNumber').val('');
    }
}

function addPoint() {

    $("#frmAddPoint").validate({
        rules: {
            pointNum: {
                required: true,
            },
            phoneNum: {
                required: true,
                minlength: 10,
            }

        },
        messages: {
            pointNum: {
                required: "Vui lòng không để trống",
            },
            phoneNum: {
                required: "Vui lòng không để trống",
                minlength: "Số Điện thoại phải >= 10 ký tự",
            }
        },
        submitHandler: function () {
            var phone = $('#mdAddPoint #txtPhoneNumber').val();
            var point = $('#mdAddPoint #txtPoint').val();
            var note = $('#mdAddPoint #txtNote').val();
            $.ajax({
                url: "/Customer/AddPoint",
                data: {
                    Phone: phone,
                    Point: point,
                    Note: note
                },
                success: function (result) {
                    if (result == SUCCESS) {
                        swal({
                            title: "Thêm Điểm Thành Công",
                            text: "",
                            icon: "success"
                        })
                        searchCustomer();
                        $('#mdAddPoint').modal("hide");
                    }
                    else {
                        swal({
                            title: "Thông Báo",
                            text: "Có lỗi.",
                            icon: "warning"
                        })
                    }
                }
            });
        }
    });
}


//function GetCustomerDetail(id) {
//    debugger;
//    $.ajax({
//        url: "/Customer/CustomerDetail",
//        data: { ID: id },
//        success: function (result) {
//            $('#View').html(result);
//        }
//    });
//}

function GetCustomerDetail(id) {
    $.ajax({
        url: "/Customer/CustomerDetailNew",
        data: { ID: id },
        success: function (result) {
            //$('#ViewDetail').html(result);
        }
    });
}

function saveEditCustomer(id) {
    $("#EditCustomer #frmEdit_Customer").validate({
        ignore: ".date",
        rules: {
            cusName: {
                required: true,
            },
            cusPhone: {
                minlength: 10,
            },
            cusEmail: {
                email: true
            }

        },
        messages: {
            cusName: {
                required: "Vui lòng không để trống",
            },
            cusPhone: {
                minlength: "Số Điện thoại phải >= 10 ký tự",
            },
            cusEmail: {
                email: "Vui lòng nhập đúng Email"
            }
        },
        submitHandler: function () {
            var name = $('#txtCusName').val().trim();
            var phone = $('#txtCusPhone').val().trim();
            var email = $('#txtCusEmail').val().trim();
            var sex = $('#cmbSex').val();
            var idImg = $("#val_id_img").val();
            //var status = $('#cbbStatusUpdate').val();
            var birthday = $('#dtpBirthDay').val().trim();
            var address = $('#txtAddress').val().trim();
            var lstUrlImg = $("#txturlImage").val().trim();

            $.ajax({
                url: "/Customer/SaveEditCustomer",
                data: {
                    Name: name,
                    Phone: phone,
                    Email: email,
                    Sex: sex,
                    LstID: idImg,
                    //Status: status,
                    BirthDay: birthday,
                    Address: address,
                    LstUrlImg: lstUrlImg,
                    ID: id
                },
                success: function (result) {
                    if (result == SUCCESS) {
                        $('#EditCustomer').modal("hide");
                        $('.modal-backdrop').hide(); // xóa lớp mờ mờ đen khi đóng modal lỗi
                        swal({
                            title: "Cập nhật Thành Công",
                            text: "",
                            icon: "success"
                        });
                        //setTimeout(function () {
                        GetCustomerDetail(id);
                        //}, 1000);

                    }
                    else {
                        swal({
                            title: "Lỗi",
                            text: "",
                            icon: "warning"
                        });
                    }
                }
            });
        }
    });
}

function SearchHistoryPoint(id) {
    var fromdate = $('#addPoint #dtpFromDate').val();
    var todate = $('#addPoint #dtpTodate').val();

    $.ajax({
        url: "/Customer/SearchHistoryPoint",
        data: {
            Page: 1,
            cusID: id,
            FromDate: fromdate,
            ToDate: todate
        },
        success: function (result) {
            $('#ListHistoryPoint').html(result);
        }
    });
}

function SearchRequset(id) {
    var fromdate = $('#changePoint #dtpFromdateRQ').val();
    var todate = $('#changePoint #dtpToDateRQ').val();

    $.ajax({
        url: "/Customer/SearchReQuest",
        data: {
            Page: 1,
            cusID: id,
            FromDate: fromdate,
            ToDate: todate
        },
        success: function (result) {
            $('#ListRequest').html(result);
        }
    });
}

function searchOrderHistory(id) {
    $.ajax({
        url: "/Customer/searchOrderHistory",
        data: {
            Page: 1,
            cusID: id,
            fromDate: $("#dtpFromdateOH").val(),
            toDate: $("#dtpToDateOH").val()
        },
        success: function (result) {
            $("#ListOrderHistory").html(result);
        }
    });
}

// Xóa Cus
function DeleteCus(id) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: "/Customer/DeleteCustomer",
                    data: { ID: id },
                    success: function (result) {
                        if (result == SUCCESS) {
                            swal({
                                title: "Xóa thành công",
                                text: "",
                                icon: "success"
                            })
                            searchCustomer();
                        } else if (result == 2) {
                            swal({
                                title: "Không thể xóa khách hàng",
                                text: "Chỉ được xóa những khách hàng tạm dừng hoạt động.",
                                icon: "warning"
                            })
                            searchCustomer();
                        }
                        else {
                            swal({
                                title: "Có lỗi",
                                text: "",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })
}


// ShowEdit RAnk
function showEditRank(id) {
    $('#editRank').remove();
    $.ajax({
        url: "/Config/ShowEditRank",
        data: { ID: id },
        success: function (result) {
            $('#editArea').html(result);
            $('#editRank').modal("show");
        }
    });
}

// Edit Rank
function saveEditRank(id) {
    var min = $('#txtMinPointEdit').val().trim();
    var max = $('#txtMaxPointEdit').val().trim();
    var des = $('#txtDesEdit').val().trim();

    swal({
        title: "Cảnh Báo !!",
        text: "Nếu bạn thay vùng giá trị điểm của mục này sẽ làm vùng giá trị khác thay đổi theo",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                if (min == "" || max == "" || des == "") {
                    swal({
                        title: "Thông báo!",
                        text: "Vui lòng nhập đầy đủ thông tin",
                        icon: "warning"
                    })
                    return;
                }
                if (min >= max) {
                    swal({
                        title: "Cảnh Báo !!",
                        text: "Giá Trị Bắt Đầu Phải Nhỏ Hơn Giá Trị Kết Thúc",
                        icon: "warning"
                    })
                    return;
                }
                $.ajax({
                    url: "/Config/EditRank",
                    data: {
                        ID: id,
                        Descripton: des,
                        MaxPoint: max,
                        MinPoint: min
                    },
                    success: function (result) {
                        if (result == SUCCESS) {
                            swal({
                                title: "Cập nhật thành công",
                                text: "",
                                icon: "success"
                            });

                            setTimeout(function () {
                                $(".swal-button--confirm").click();
                            }, 1000);
                            setTimeout(function () {
                                LoadRank();
                            }, 1500);
                            $("#editRank").modal("hide");

                        }
                        else {
                            swal({
                                title: "Có lỗi",
                                text: "",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })

}

function getConfigGiftDetail(id) {
    //swal({
    //    title: "Thông báo",
    //    text: "Chức năng đang xây dựng",
    //    icon: "warning"
    //})
    $.ajax({
        url: '/Config/GetConfigGiftDetail',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#divConfigGiftDetail").html(response);
            $('#configGiftDetail').modal('show');
        }
    });
}

function getConfigCardDetail(id) {
    $.ajax({
        url: '/Config/GetConfigCardDetail',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#divConfigCardDetail").html(response);
            $('#updateConfigCard').modal('show');
        }
    });
}


function searchPoint() {
    $fromDate = $('#fromDate').val().trim();
    $toDate = $('#toDate').val().trim();
    $keySearch = $.trim($('#keySearch').val());
    $type = $("#type").val();
    $.ajax({
        url: "/Point/Search",
        data: {
            Page: 1,
            Type: $type,
            KeySearch: $keySearch,
            fromDate: $fromDate,
            toDate: $toDate
        },
        success: function (response) {
            $('#tablePoint').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function getPointDetail($id) {
    $.ajax({
        url: "/Point/GetPointDetail",
        data: { ID: $id },
        type: 'POST',
        success: function (response) {
            $('#modalDetailPoint').html(response);
            $('#detailPoint').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function searchWarrantyCard() {
    $fromDate = $('#fromDate').val();
    $toDate = $('#toDate').val();
    $status = $('#status').val();
    $code = $.trim($('#warrantyCardCode').val());

    if ($code.length == 16) {
        var count = $code.length - 1;
        $warrantyCardCode = $code.substring(0, count);
    } else {
        $warrantyCardCode = $code;
    }

    $.ajax({
        url: "/Warranty/Search",
        data: { Page: 1, fromdate: $fromDate, ToDate: $toDate, Status: $status, WarrantyCardCode: $warrantyCardCode },
        success: function (response) {
            $('#TableWarranty').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function CreateWarranty() {
    var quantity = $.trim($('#quantity').val());
    var point = $.trim($('#point').val());
    var expireDate = $('#expireDate').val();
    if (quantity == "" || point == "" || expireDate == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        if (!isNumeric(quantity)) {
            swal({
                title: "Thông báo",
                text: "Số lượng chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        }
        else {
            if (!isNumeric(point)) {
                swal({
                    title: "Thông báo",
                    text: "Số điểm chỉ được phép nhập số!",
                    icon: "warning"
                })
                return;
            }
            else {
                $.ajax({
                    url: "/Warranty/CreateWarranty",
                    data: $('#form_create_warranty').serialize(),
                    type: "POST",
                    success: function (response) {
                        if (response != null) {
                            $('#QrCodeWarrantyCard').html(response);
                            $('#createWarranty').modal('hide');
                            swal({
                                title: "Thông báo",
                                text: "Thành công!",
                                icon: "success"
                            });

                            $('#printWarranty').modal('show');
                            searchWarrantyCard();
                            //resetInputCreteWrtCard();
                        } else {
                            //resetInputCreteWrtCard();
                            swal({
                                title: "Thông báo",
                                text: "Lỗi",
                                icon: "warning"
                            })
                            return;
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        }
    }
}

function searchRequestForGift() {
    var name = $('#txtCusName').val().trim();
    var type = $('#slGiftType').val();
    var fromdate = $('#txtFromDate').val().trim();
    var todate = $('#txtToDate').val().trim();

    $.ajax({
        url: "/StatisticGift/SearchRequestForGift",
        data: {
            Page: 1,
            CusName: name,
            GiftType: type,
            FromDate: fromdate,
            ToDate: todate
        },
        success: function (result) {
            $('#TableRQ').html(result);
        }
    });
}

function resetInputCreteWrtCard() {
    $('#quantity').val('');
    $('#point').val('');
}

function DeleteWarrantyCard($id) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Warranty/DeleteWarrantyCard',
                    data: { ID: $id },
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Thông báo",
                                text: "Xóa thành công!",
                                icon: "success"
                            });
                            searchWarrantyCard();
                        }
                        else {
                            swal({
                                title: "Thông báo",
                                text: "Lỗi!",
                                icon: "warning"
                            });
                        }
                    }
                });
            }
        })
}

function getWarrantyDetail($ID, $WarrantyCardCode) {
    $.ajax({
        url: '/Warranty/getWarrantyDetail/',
        data: { ID: $ID, WarrantyCodeCard: $WarrantyCardCode },
        success: function (response) {
            $('#DetailWarrantyQRCode').html(response);
            $('#QrcodeDetail').modal('show');
        }
    });
}


// thống kê  doanh thu
function statisticRevenue() {
    var obj = $('#slObj').val();
    var fd = $('#txtFromDate').val();
    var td = $('#txtToDate').val();

    $.ajax({
        url: "/StatisticRevenue/Search",
        data: {
            Page: 1,
            obj: obj,
            FromDate: fd,
            ToDate: td
        },
        success: function (result) {
            $('#list').html(result);
        }
    });
}

// Tìm Kiếm Đơn Hàng
function searchOrder() {
    var status = $('#slStatus').val();
    var fd = $('#txtFromDate').val();
    var td = $('#txtToDate').val();
    var paymentType = $('#slPaymentType').val();
    debugger
    $.ajax({
        url: "/Order/Search",
        data: {
            Page: 1,
            Status: status,
            FromDate: fd,
            ToDate: td,
            PaymentType: paymentType,
            Phone: $('#txtCusPhone').val().trim()
        },
        success: function (result) {
            $('#list').html(result);
        }
    });
}

//Tìm kiếm sản phẩm khuyến mại
function SearchBanner() {
    var fromDate = $('#fromDateBanner').val();
    var toDate = $('#toDateBanner').val();
    var titleBanner = $.trim($('#titleBanner').val());
    var status = $('#slStatus').val();
    var typeSend = $('#slTypeSend').val();
    var type = $('#slType').val();
    $.ajax({
        url: '/Banner/SearchBanner',
        data: {
            Page: 1,
            fromDate: fromDate,
            toDate: toDate,
            title: titleBanner,
            status: status,
            typeSend: typeSend,
            type: type,
        },
        success: function (response) {
            $('#tableBanner').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
//Tìm kiếm sản phẩm
function SearchItemSale() {
    var fromDateCreate = $('#fromDateItemSale').val();
    var toDateCreate = $('#toDateItemSale').val();
    var itemSaleName = $.trim($('#itemSaleName').val());
    var category = $('#cbbCategorySearch').val();
    $.ajax({
        url: '/ItemSale/SearchItemSaleWeb',
        data: {
            Page: 1,
            fromDateCreate: fromDateCreate,
            toDateCreate: toDateCreate,
            itemSaleName: itemSaleName,
            category: category,
        },
        success: function (response) {
            $('#tableItemSale').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
//Tìm kiếm sản phẩm
function SearchItem() {
    var fromDate = $('#fromDateItem').val();
    var toDate = $('#toDateItem').val();
    var itemName = $.trim($('#itemName').val());
    var category = $('#cbbCategorySearch').val();
    var status = $('#slStatus').val();
    var isHot = $('#slIsHot').val();
    $.ajax({
        url: '/Item/SearchWeb',
        data: {
            Page: 1,
            fromDate: fromDate,
            toDate: toDate,
            itemName: itemName,
            category: category,
            Status: status,
            IsHot: isHot,
        },
        success: function (response) {
            $('#tableItem').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
//tim kieems voucher
function SearchVoucherCustomer() {
    var fromDateVoucher = $('#fromDateVoucher').val();
    var toDateVoucher = $('#toDateVoucher').val();
    var nameVoucher = $.trim($('#nameVoucher').val());
    var statusVoucher = $('#slStatusVoucher').val();
    var typeVoucherRank = $('#slTypeVoucherRank').val();
    var typeDiscount = $('#slTypeDiscount').val();
    $.ajax({
        url: '/VoucherCustomer/SearchVoucherCustomer',
        data: {
            Page: 1,
            nameVoucher: nameVoucher,
            fromDate: fromDateVoucher,
            toDate: toDateVoucher,
            typeDiscount: typeDiscount,
            status: statusVoucher,
            rank: typeVoucherRank,
        },
        success: function (response) {
            $('#tableVoucherCustomer').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
//voucher giới thiệu
function SearchVoucherIntroduce() {
    var fromDateVoucher = $('#fromDateVoucherIntroduce').val();
    var toDateVoucher = $('#toDateVoucherIntroduce').val();
    var nameVoucher = $.trim($('#nameVoucherIntroduce').val());
    var typeDiscount = $('#slTypeDiscountVoucherIntroduce').val();
    $.ajax({
        url: '/VoucherIntroduce/SearchVoucherIntroduce',
        data: {
            Page: 1,
            nameVoucher: nameVoucher,
            fromDate: fromDateVoucher,
            toDate: toDateVoucher,
            typeDiscountVoucherIntroduce: typeDiscount,
        },
        success: function (response) {
            $('#tableVoucherIntroduce').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
function listNameItemSale(categryId, id) {
    var valueSelectItemSale = $("#" + id).attr("value");

    $.ajax({
        url: "/ItemSale/LoadNameItem",
        data: { CategryId: categryId },
        success: function (result) {
            $("#" + id).empty();
            $("#" + id).append("<option value=''>Tên sản phẩm</option>");
            $.each(result, function () {
                var select = "<option value='" + this.ID + "'>" + this.Name + "</option>";
                $("#" + id).append(select);
            });
            //selected ten theo danh muc
            if (valueSelectItemSale != "" && valueSelectItemSale != undefined) {
                $("#" + id).val(valueSelectItemSale);
            }
        }
    });
}
function enableCheckbox(idTypeSend, id) {
    if (idTypeSend == 0) {
        $("#checkMes").children().prop('disabled', true);
        $('#checkbox-value').text($('#_checkMes').val());
        $("#checkMes").children().prop('checked', false);
        $('#_checkMes').val("0");
    }
    else {
        $("#checkMes").children().prop('disabled', false);
    }
}

function enableTypeMoney(slTypeDiscountCreate, id) {
    if (slTypeDiscountCreate == 1) {
        $("#textMoneyB").remove();
        $("#textMoneyA").append('<div class="col-md-1 col-1 mt-4" id="textMoneyB"><a id="textMoney2">VNĐ</a></div>');
    }
    else {
        $("#textMoneyB ").remove();
        $("#textMoneyA").append('<div class="col-md-1 col-1 mt-4" id="textMoneyB"><a id="textMoney2">%</a></div>');
    }
}
function changeTextEdit(typeDiscountVouEdit, id) {
    if (typeDiscountVouEdit == 1) {
        $("#textMoney1").remove();
        $("#textMoney2").append('<div class="col-md-1 col-1 mt-4" id="textMoney1"><a id="textMoney2">VNĐ</a></div>');
    }
    else {
        $("#textMoney1 ").remove();
        $("#textMoney2").append('<div class="col-md-1 col-1 mt-4" id="textMoney1"><a id="textMoney2">%</a></div>');
    }
}

function enableCheckboxEdit(idTypeSend, id) {
    if (idTypeSend == 0) {
        $("#checkMesEdit").children().prop('disabled', true);
        $('#checkbox-value').text($('#checkMesEdit').val());
        $("#checkMesEdit").children().prop('checked', false);
        $('#checkMesEdit').val("0");
    }
    else {
        $("#checkMesEdit").children().prop('disabled', false);
    }
}

function CreateItem() {
    var CategoryID = $('#cbbCategory').val();
    //var Status = $('#StatusCreate').val();
    var Code = $('#CodeCreate').val().trim();
    var Name = $('#NameCreate').val().trim();
    var Price = $('#PriceCreate').val().trim();
    //var Image = $('#txt-url-img').val().trim();  
    var Note = $.trim(CKEDITOR.instances['NoteCreate'].getData());
    var isHot = $('#slIsHot').val();
    //var technical = $.trim(CKEDITOR.instances['technicalCreate'].getData());

    //var warranty = $("#warrantyCreate").val().trim().replace(/,/g, "");
    var special;
    var Image = "";
    $.each($('.imgs'), function () {
        Image += $(this).attr('src') + ',';
        //index = Image.lastIndexOf('files/');
        //Image = Image.slice(index + 6);
    });
    Image = Image.slice(0, Image.length - 1);
    if ($('#sendNotify').is(":checked")) {
        special = 1;
    } else {
        special = 0;
    }
    if ($('#sendNotify').is(":checked")) {
        special = 1;
    } else {
        special = 0;
    }

    //if (technical.length == 0) {
    //    swal({
    //        title: "Thông báo",
    //        text: "Vui lòng nhập chi tiết sản phẩm!",
    //        icon: "warning"
    //    })
    //    return;
    //}

    if (Code.length < 6) {
        swal({
            title: "Thông báo",
            text: "Mã sản phẩm phải dài tối thiểu 6 ký tự!",
            icon: "warning"
        })
        return;
    }
    if (isSpace(Code)) {
        swal({
            title: "Thông báo",
            text: "Mã sản phẩm không được có khoảng trắng!",
            icon: "warning"
        })
        return;
    }
    if (Code == "" || Name == "" || Price == "" || isHot == "Sản phẩm bán chạy" || Image == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        if (!isNumeric(Price)) {
            swal({
                title: "Thông báo",
                text: "Giá tiền chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        }
        else {
            $.ajax({
                url: '/Item/CreateItem',
                data: {
                    CategoryID: CategoryID,
                    Code: Code,
                    Name: Name,
                    Price: Price,
                    Note: Note,
                    ImageUrl: Image,
                    Special: special,
                    IsHot: isHot,
                    //Tech: technical

                },
                type: 'GET',
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thành công!",
                            text: "",
                            icon: "success"
                        });
                        $('#createItem').modal('hide');
                        window.location = "/Item/Index";
                        SearchItem();
                    }
                    else if (response == EXISTING) {
                        swal({
                            title: "Không thể tạo sản phẩm",
                            text: "Mã sản phẩm đã tồn tại. Vui lòng sử dụng mã khác.",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Có lỗi xảy ra!",
                            text: "Không thể tạo sản phẩm",
                            icon: "warning"
                        });
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
    }
}

//Load data to Popup UpdateItem
function LoadItemSale($id) {
    $.ajax({
        url: '/ItemSale/LoadItemSale',
        data: { ID: $id },
        success: function (response) {
            $('#UpdateItemSale').html(response);
            $('#EditItemSale').modal('show');
        }
    });
}
//Load data to Popup UpdateItem
function LoadItem($id) {
    $.ajax({
        url: '/Item/LoadItem',
        data: { ID: $id },
        success: function (response) {
            $('#UpdateItem').html(response);
            $('#EditItem').modal('show');
        }
    });
}

function DeleteItemSale($id) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((isconfirm) => {
        if (isconfirm) {
            $.ajax({
                url: '/ItemSale/DeleteItemSale',
                data: { ID: $id },
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thông báo",
                            text: "Xóa thành công!",
                            icon: "success"
                        });
                        window.location = "/ItemSale/Index";
                        SearchItem();
                    }
                    else {
                        if (response == 3) {
                            swal({
                                title: "Đã tồn tại lô hàng của sản phẩm này, không thể xóa!",
                                text: "",
                                icon: "warning"
                            });
                        }
                        else {
                            swal({
                                title: "Thông báo",
                                text: "Lỗi!",
                                icon: "warning"
                            });
                        }
                    }
                }
            });
        }
    })
}
function DeleteItem($id) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((isconfirm) => {
        if (isconfirm) {
            $.ajax({
                url: '/Item/DeleteItem',
                data: { ID: $id },
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thông báo",
                            text: "Xóa thành công!",
                            icon: "success"
                        });
                        window.location = "/Item/Index";
                        SearchItem();
                    }
                    else {
                        if (response == 3) {
                            swal({
                                title: "Đã tồn tại lô hàng của sản phẩm này, không thể xóa!",
                                text: "",
                                icon: "warning"
                            });
                        }
                        else {
                            swal({
                                title: "Thông báo",
                                text: "Lỗi!",
                                icon: "warning"
                            });
                        }
                    }
                }
            });
        }
    })
}

function DeleteBanner($id) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((isconfirm) => {
        if (isconfirm) {
            $.ajax({
                url: '/Banner/DeleteBanner',
                data: { ID: $id },
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thông báo",
                            text: "Xóa thành công!",
                            icon: "success"
                        });
                        window.location = "/Banner/Index";
                        SearchBanner();
                    }
                    else {
                        swal({
                            title: "Thông báo",
                            text: "Lỗi!",
                            icon: "warning"
                        });
                    }
                }
            });
        }
    })
}

function DeleteVoucherCustomer($id) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((isconfirm) => {
        if (isconfirm) {
            $.ajax({
                url: '/VoucherCustomer/DeleteVoucherCustomer',
                data: { ID: $id },
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thông báo",
                            text: "Xóa thành công!",
                            icon: "success"
                        });
                        window.location = "/VoucherCustomer/Index";
                        SearchVoucherCustomer();
                    }
                    else {
                        swal({
                            title: "Thông báo",
                            text: "Lỗi!",
                            icon: "warning"
                        });
                    }
                }
            });
        }
    })
}

function DeleteVoucherIntroduce($id) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((isconfirm) => {
        if (isconfirm) {
            $.ajax({
                url: '/VoucherIntroduce/DeleteVoucherIntroduce',
                data: { ID: $id },
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thông báo",
                            text: "Xóa thành công!",
                            icon: "success"
                        });
                        window.location = "/VoucherIntroduce/Index";
                        SearchVoucherIntroduce();
                    }
                    else if (response == 421) {
                        swal({
                            title: "Thông báo",
                            text: "Lỗi, Voucher đang kích hoạt không thể xóa! ",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Thông báo",
                            text: "Lỗi!",
                            icon: "warning"
                        });
                    }
                }
            });
        }
    })
}

function CreateItemSale() {
    var categoty = $('#lsCategory').val()
    var itemId = $('#slNameItemSale').val()
    var quantity = $('#quantity').val()
    var PriceSale = $('#PriceSale').val()

    var timeFrom = $('#timeFromDateSale').val()
    var fromDateSale = $('#fromDateSale').val()
    var datetimeFrom = fromDateSale + " " + timeFrom;

    var timeTo = $('#timeToDateSale').val()
    var toDateSale = $('#toDateSale').val()
    var datetimeTo = toDateSale + " " + timeTo;

    if (categoty == "" || itemId == "" || quantity == "" || PriceSale == "" || timeFrom == "" || fromDateSale == "" || timeTo == "" || toDateSale == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        if (!isNumeric(PriceSale)) {
            swal({
                title: "Thông báo",
                text: "Giá tiền chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        }
        else {
            $.ajax({
                url: '/ItemSale/CreateItemSale',
                data: {
                    CategoryID: categoty,
                    itemId: itemId,
                    quantity: quantity,
                    Price: PriceSale,
                    fromDateTime: datetimeFrom,
                    toDateTime: datetimeTo,
                },
                type: 'GET',
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thành công!",
                            text: "",
                            icon: "success"
                        });
                        window.location = "/ItemSale/Index";
                        SearchItem();
                    }
                    else if (response == EXISTING) {
                        swal({
                            title: "Không thể tạo sản phẩm khuyến mại",
                            text: "Sản phẩm đã tồn tại. Vui lòng sử dụng mã khác.",
                            icon: "warning"
                        });
                    }
                    else if (response == 413) {
                        swal({
                            title: "Không thể tạo sản phẩm khuyến mại",
                            text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc. Vui lòng chọn lại khoảng thời gian khác.",
                            icon: "warning"
                        });
                    }
                    else if (response == 414) {
                        swal({
                            title: "Không thể tạo sản phẩm khuyến mại",
                            text: "Thời gian khuyến mại không hợp lệ. Vui lòng chọn lại khoảng thời gian khác.",
                            icon: "warning"
                        });
                    }
                    else if (response == 415) {
                        swal({
                            title: "Không thể tạo sản phẩm khuyến mại",
                            text: "Giá khuyến mại không được lớn hơn giá gốc. Vui lòng kiểm tra lại giá.",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Có lỗi xảy ra!",
                            text: "Không thể tạo sản phẩm khuyến mại",
                            icon: "warning"
                        });
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
    }
}

function SaveEditConfig() {
    var valuePoint = $('#valuePoint').val();
    if (valuePoint == "") {
        valuePoint = 0;
    } else {
        $.ajax({
            url: '/Config/SaveEditConfig',
            data: {
                valuePoint: valuePoint,
            },
            success: function (response) {
                if (response == SUCCESS) {
                    swal({
                        title: "Thành công!",
                        text: "",
                        icon: "success"
                    });
                    window.location = "/Config/Index";
                } else if (response == 423) {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Điểm nhận được theo giá trị đợi hàng không được lớn hơn 100%",
                        icon: 'error',
                    })
                }
                else {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Không thể sửa sản phẩm.",
                        icon: "warning"
                    });
                }
            },
            error: function (result) {
                console.log(result.responseText);
            }
        });
    }
}

//Lưu lại cập nhập

function SaveEditItem() {
    var ID = $('#ID').val();
    var CategoryID = $('#cbbCategoryUpdate').val();
    var Code = $('#CodeEdit').val().trim();
    var Name = $('#NameEdit').val().trim();
    var Status = $('#StatusCreate').val();
    var IsHot = $('#slIsHotEdit').val();
    var Price = $('#PriceEdit').val().trim();
    //var Image = $('#tagImage').attr('src');
    //var ImageUrl = $('#txt-url-image').val();
    //var Warranty = $('#warrantyEdit').val();
    var Note = $.trim(CKEDITOR.instances['NoteEdit'].getData());
    var ImageUrl = "";
    $.each($('._lstImage'), function () {
        ImageUrl += $(this).attr('src') + ',';
        //index = ImageUrl.lastIndexOf('files/');
        //ImageUrl = ImageUrl.slice(index + 6);
    });
    ImageUrl = ImageUrl.slice(0, ImageUrl.length - 1);
    //var technical = $.trim(CKEDITOR.instances['technicalEdit'].getData());
    var special;
    if ($('#sendNotify').is(":checked")) {
        special = 1;
    } else {
        special = 0;
    }
    if (Code == "" || Name == "" || Price == "" || ImageUrl == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        if (!isNumeric(Price)) {
            swal({
                title: "Thông báo",
                text: "Giá tiền chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        }
        else {
            $.ajax({
                url: '/Item/SaveEditItem',
                data: {
                    ID: ID,
                    Code: Code,
                    Name: Name,
                    Status: Status,
                    IsHot: IsHot,
                    CategoryID: CategoryID,
                    ImageUrl: ImageUrl,
                    //Tech: technical,
                    Note: Note,
                    //Warranty: warranty,
                    Price: Price,
                    //Special: special
                },
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thành công!",
                            text: "",
                            icon: "success"
                        });
                        $('#EditItem').modal('hide');
                        window.location = "/Item/Index";
                        SearchItem();
                    }
                    else if (response == 417) {
                        swal({
                            title: "Có lỗi xảy ra!",
                            text: "Không thể sửa danh mục sản phẩm.",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Có lỗi xảy ra!",
                            text: "Không thể sửa sản phẩm.",
                            icon: "warning"
                        });
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
    }
}
//Cập nhật customer
function SaveEditCus(ID) {
    var _rank = $('#cmbRankDetailCus').val()
    var _status = $('#slStatusCus').val()
    $.ajax({
        url: '/Customer/SaveEditCustomer',
        data: {
            ID: ID,
            RankCus: _rank,
            StatusCus: _status,
        },
        success: function (response) {
            if (response == SUCCESS) {
                swal({
                    title: "Thành công!",
                    text: "",
                    icon: "success"
                });
                window.location = "/Customer/Index";
                searchCustomer();
            }
            else {
                swal({
                    title: "Có lỗi xảy ra!",
                    text: "Không thể sửa thông tin khách hàng.",
                    icon: "warning"
                });
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
function SaveEditOrderNew(ID) {
    var _status = $('#slStatusEdit').val()
    $.ajax({
        url: '/Order/SaveEditOrderNew',
        data: {
            ID: ID,
            StatusOrder: _status,
        },
        success: function (response) {
            if (response == SUCCESS) {
                swal({
                    title: "Thành công!",
                    text: "",
                    icon: "success"
                });
                window.location = "/Order/Index";
                searchOrder();
            } else if (response == 502) {
                swal({
                    title: "Có lỗi xảy ra!",
                    text: "Đơn hàng không tồn tại.",
                    icon: "warning"
                });
            } else if (response == 503) {
                swal({
                    title: "Có lỗi xảy ra!",
                    text: "Trạng thái đơn hàng đã bị thay đổi.",
                    icon: "warning"
                });
            }
            else {
                swal({
                    title: "Có lỗi xảy ra!",
                    text: "Không thể sửa.",
                    icon: "warning"
                });
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
//Lưu lại cập nhập

function SaveEditItemSale(ID) {
    var categoty = $('#lsCategoryUpdate').val()
    var itemId = $('#slNameItemSaleUpdate').val()
    var quantity = $('#QuantityEdit').val()
    var PriceSale = $('#PriceEdit').val()

    var timeFrom = $('#timeFromDateSaleUpdate').val()
    var fromDateSale = $('#fromDateSaleUpdate').val()
    var datetimeFrom = fromDateSale + " " + timeFrom;

    var timeTo = $('#timeToDateSaleUpdate').val()
    var toDateSale = $('#toDateSaleUpdate').val()
    var datetimeTo = toDateSale + " " + timeTo;

    if (categoty == "" || itemId == "" || quantity == "" || PriceSale == "" || timeFrom == "" || fromDateSale == "" || timeTo == "" || toDateSale == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        if (!isNumeric(PriceSale)) {
            swal({
                title: "Thông báo",
                text: "Giá tiền chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        }
        else {
            $.ajax({
                url: '/ItemSale/SaveEditItemSale',
                data: {
                    ID: ID,
                    CategoryID: categoty,
                    itemId: itemId,
                    quantity: quantity,
                    Price: PriceSale,
                    fromDateTime: datetimeFrom,
                    toDateTime: datetimeTo,
                },
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Thành công!",
                            text: "",
                            icon: "success"
                        });
                        $('#EditItemSale').modal('hide');
                        window.location = "/ItemSale/Index";
                        SearchItem();
                    }
                    else if (response == 414) {
                        swal({
                            title: "Không thể tạo sản phẩm khuyến mại",
                            text: "Thời gian khuyến mại không hợp lệ. Vui lòng sử dụng thời gian khác.",
                            icon: "warning"
                        });
                    }
                    else if (response == 415) {
                        swal({
                            title: "Không thể tạo sản phẩm khuyến mại",
                            text: "Giá khuyến mại không được lớn hơn giá gốc. Vui lòng kiểm tra lại giá.",
                            icon: "warning"
                        });
                    }
                    else if (response == 416) {
                        swal({
                            title: "Không thể tạo sản phẩm khuyến mại",
                            text: "Số lượng sản phẩm khuyến mại không được nhỏ hơn số lượng đã bán. Vui lòng kiểm tra lại.",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Có lỗi xảy ra!",
                            text: "Không thể sửa sản phẩm.",
                            icon: "warning"
                        });
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
    }
}
function SaveEditBanner(ID) {
    var _title = $('#titleEdit').val()
    var typeSend = $('#slTypeSendEdit').val()
    var type = $('#slTypeEdit').val()
    var icheck = $('#_checkMesEdit').val()
    var status = $('#slStatusEdit').val()
    var imageUrl = $('#AddImgLogoPlace2').attr('src');
    var _content1 = $.trim(CKEDITOR.instances['NoteEditBanner'].getData());
    //var _content1 = "aaaa";

    if (_title == "" || typeSend == "" || type == "" || imageUrl == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        $.ajax({
            url: '/Banner/SaveEditBanner',
            data: {
                ID: ID,
                title: _title,
                typeSend: typeSend,
                type: type,
                imageUrl: imageUrl,
                content: _content1,
                icheck: icheck,
                status: status
            },
            success: function (response) {
                if (response == SUCCESS) {
                    swal({
                        title: "Thành công!",
                        text: "",
                        icon: "success"
                    });
                    $('#EditBanner').modal('hide');
                    window.location = "/Banner/Index";
                    SearchBanner();
                }
                else {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Không thể sửa banner.",
                        icon: "warning"
                    });
                }
            },
            error: function (result) {
                console.log(result.responseText);
            }
        });
    }
}
function SaveVoucherCustomer(ID) {
    var statusVCEdit = $('#slStatusVoucherCustomerEdit').val();
    var quantityVCEdit = $('#quantityVoucherCustomerEdit').val();
    var fromDateVCEdit = $('#fromDateVoucherEdit').val();
    var toDateVCEdit = $('#toDateVoucherEdit').val();
    var imgVC = $('#AddImgLogoPlace').attr('src');
    var NoteVCEdit = $.trim(CKEDITOR.instances['NoteVoucherCustomerEdit'].getData());

    if (statusVCEdit == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        $.ajax({
            url: '/VoucherCustomer/SaveEditVoucherCustomer',
            data: {
                ID: ID,
                statusVouEdit: statusVCEdit,
                quantityVouEdit: quantityVCEdit,
                fromDateVoucherEdit: fromDateVCEdit,
                toDateVoucherEdit: toDateVCEdit,
                imgVou: imgVC,
                NoteVoucherEdit: NoteVCEdit
            },
            success: function (response) {
                if (response == SUCCESS) {
                    swal({
                        title: "Thành công!",
                        text: "",
                        icon: "success"
                    });
                    window.location = "/VoucherCustomer/Index";
                    SearchVoucherCustomer();
                }
                else if (response == 414) {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Ngày bắt đầu lớn hơn ngày kết thúc",
                        icon: "warning"
                    });
                }
                else if (response == 416) {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Số lượng quy định không được nhỏ hơn số lượng đã sử dụng. Vui lòng kiểm tra lại.",
                        icon: "warning"
                    });
                }
                else {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Không thể sửa.",
                        icon: "warning"
                    });
                }
            },
            error: function (result) {
                console.log(result.responseText);
            }
        });
    }
}
function SaveVoucherIntroduce(ID) {
    var imgVC = $('#AddImgLogoPlace').attr('src');
    var NoteVCEdit = $.trim(CKEDITOR.instances['NoteVouEdit'].getData());
    var status = $('#slStatusVouEdit').val();
    var nameVouEdit = $('#nameVouEdit').val().trim();
    var typeDiscountVouEdit = $('#typeDiscountVouEdit').val();
    var discountVouEdit = $('#discountVouEdit').val();
    if (status == "" || nameVouEdit == "" || typeDiscountVouEdit == "" || discountVouEdit == "" || imgVC == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        $.ajax({
            url: '/VoucherIntroduce/SaveEditVoucherIntroduce',
            data: {
                ID: ID,
                imgVoucherIntroduceEdit: imgVC,
                NoteVoucherIntroduceEdit: NoteVCEdit,
                status: status,
                nameVouEdit: nameVouEdit,
                typeDiscountVouEdit: typeDiscountVouEdit,
                discountVouEdit: discountVouEdit,
            },
            success: function (response) {
                if (response == SUCCESS) {
                    swal({
                        title: "Thành công!",
                        text: "",
                        icon: "success"
                    });
                    window.location = "/VoucherIntroduce/Index";
                    SearchVoucherCustomer();
                }
                else if (response == 414) {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Ngày bắt đầu lớn hơn ngày kết thúc",
                        icon: "warning"
                    });
                } else if (response == 423) {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Mức giảm không được lớn hơn 100%",
                        icon: 'error',
                    })
                }
                else {
                    swal({
                        title: "Có lỗi xảy ra!",
                        text: "Không thể sửa.",
                        icon: "warning"
                    });
                }
            },
            error: function (result) {
                console.log(result.responseText);
            }
        });
    }
}
// Show Edit Order
function showEditOrder(id) {

    $.ajax({
        url: "/Order/ShowEditOrder",
        data: { ID: id },
        success: function (result) {
            $('#fillModal').html(result);
            $('#mdEdit').modal("show");
        }
    });
}
//Xuất mã barcode lô hàng
function ExportBarcode(id) {
    $('#modalLoad').modal("show");
    // window.location = "ExportQR?batchID=" + id;
    $.ajax({
        url: window.location = "ExportQR?batchID=" + id,
        success: function (response) {
            $('#modalLoad').modal("hide");
            $('#mdBatchDetail').modal("hide");
        }
    });
}
// Save Edit Status Order
function SaveEditOrder(id) {

    $("#frmEditOrder").validate({
        rules: {
            CusName: {
                required: true
            },
            CusPhone: {
                //required: true,
                number: true,
                minlength: 10
            },
            CusAddress: {
                required: true
            },
            //AddPoint: {
            //    required: true,
            //    number: true,
            //    minlength: 1
            //},
            Discount: {
                required: true,
                number: true
            }
        },
        messages: {
            CusName: {
                required: "Vui lòng không để trống"
            },
            CusPhone: {
                //required: "Vui lòng không để trống",
                number: "Vui lòng nhập số",
                minlength: "Số ĐT phải có ít nhất 10 chứ số"
            },
            CusAddress: {
                required: "Vui lòng không để trống"
            },
            //AddPoint: {
            //    required: "Vui lòng không để trống",
            //    number: "Vui lòng nhập số",
            //    minlength: "Không chấp nhận số âm"
            //},
            Discount: {
                required: "Vui lòng không để trống",
                number: "Vui lòng nhập số"
            }
        },
        submitHandler: function () {
            var status = $('#mdEdit #slStatus').val();
            //var addPoint = $("#mdEdit #txtAddPoint").val().trim().replace(/,/g, "");
            var buyerName = $("#mdEdit #txtCusName").val().trim();
            var buyerPhone = $("#mdEdit #txtPhone").val().trim();
            var buyerAddress = $("#mdEdit #txtAddress").val().trim();
            //var disCount = $("#mdEdit #txtDiscount").val().replace(/,/g, '');
            var totalPrice = cms_decode_currency_format($("#mdEdit #Pay").html());
            var discount = cms_decode_currency_format($("#mdEdit #textMoneyDiscount").html());
            $('#mdEdit').modal('hide');
            $("#modalLoad").modal("show");
            var note = $("#Note").val().trim();
            $.ajax({
                url: "/Order/SaveEditOrder",
                data: {
                    ID: id,
                    Status: status,
                    AddPoint: addPoint,
                    BuyerName: buyerName,
                    BuyerPhone: buyerPhone,
                    BuyerAddress: buyerAddress,
                    TotalPrice: totalPrice,
                    Discount: discount,
                    Note: note
                },
                success: function (result) {
                    if (result == SUCCESS) {
                        //$('#mdEdit').modal('hide');
                        $("#modalLoad").modal("hide");
                        window.location = "/Order/Index";
                        swal({
                            title: "Thông báo",
                            text: "Cập nhật thành công",
                            icon: "success"
                        });
                        setTimeout(function () {
                            searchOrder();
                        }, 1000);

                    }
                    else {
                        //$('#mdEdit').modal('hide');
                        swal({
                            title: "Thông báo",
                            text: "Có Lỗi !!",
                            icon: "warning"
                        });
                    }
                }
            });
        }
    });
}


// delete order
function deleteOrder(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    else {
        swal({
            title: "Bạn chắc chắn xóa chứ?",
            text: "",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((isConFirm) => {

            if (isConFirm) {
                $.ajax({
                    url: '/Order/DeleteOrder',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Xóa thành công!",
                                text: "",
                                icon: "success"
                            })
                            window.location = "/Order/Index";
                            searchOrder();
                        } else {
                            swal({
                                title: "Không thể xóa!",
                                text: "Có lỗi.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }

        })
    }
}


// Search Agent
function searchAgent() {
    var code = $('#txtCode').val().trim();
    var status = $('#slStatus').val();
    var fd = $('#txtFromDate').val();
    var td = $('#txtToDate').val();

    $.ajax({
        url: "/Agent/Search",
        data: {
            Page: 1,
            Code: code,
            Status: status,
            FromDate: fd,
            ToDate: td
        },
        success: function (result) {
            $('#list').html(result);
        }
    });
}

function createAgent() {
    $("#frmCreate").validate({
        rules: {
            Code: { required: true },
            //  Name: { required: true },
            // Address: { required: true }
        },
        messages: {
            Code: { required: "Vui lòng không để trống" },
            //  Name: { required: "Vui lòng không để trống" },
            // Address: { required: "Vui lòng không để trống" }
        },
        submitHandler: function () {
            $.ajax({
                url: "/Agent/CreateAgent",
                data: $('#frmCreate').serialize(),
                success: function (result) {
                    if (result == SUCCESS) {
                        swal({
                            title: "Thêm Thành Công ",
                            text: "",
                            icon: "success"
                        });
                        window.location = "/Agent/Index";
                        $('#mdAdd').modal("hide");
                        setTimeout(function () {
                            searchAgent();
                        }, 1000);
                    }
                    else
                        if (result == EXISTING) {
                            swal({
                                title: "Không thể tạo đại lý.",
                                text: "Mã đại lý đã tồn tại. Vui lòng dùng mã khác.",
                                icon: "warning"
                            })
                        }
                        else {
                            swal({
                                title: "Thất Bại, Có Lỗi ! ",
                                text: "",
                                icon: "warning"
                            });
                        }
                }
            })
        }
    });
}

// Show Edit Agent
function showEditAgent(id) {
    $.ajax({
        url: "/Agent/ShowEditForm",
        data: { ID: id },
        success: function (result) {
            $('#fill').html(result);
            $('#mdEdit').modal("show");
        }
    });
}

// Save Edit Agent

function saveEditAgent(id) {
    $("#frmUpdate").validate({
        rules: {
            //  Name: { required: true },
            Address: { required: true }
        },
        messages: {
            //  Name: { required: "Vui lòng không để trống" },
            Address: { required: "Vui lòng không để trống" }
        },
        submitHandler: function () {
            $.ajax({
                url: "/Agent/SaveEdit",
                data: {
                    ID: id,
                    Name: $('#txtNameEdit').val().trim(),
                    Address: $('#txtAddressEdit').val().trim()
                },
                success: function (result) {
                    if (result == SUCCESS) {
                        swal({
                            title: "Cập Nhật Thành Công ",
                            text: "",
                            icon: "success"
                        });
                        window.location = "/Agent/Index";
                        $('#mdEdit').modal("hide");
                        setTimeout(function () {
                            searchAgent();
                        }, 1500);
                    }
                    else if (result = -1) {
                        swal({
                            title: "Thất Bại, Không tìm thấy SĐT  !! ",
                            text: "",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Thất Bại, Có lỗi !! ",
                            text: "",
                            icon: "warning"
                        });
                    }
                }
            })
        }
    });
}

// Delete Agent
function deleteAgent(id, customerActiveID) {
    // var customerActive = $("#customerActiveID").val();
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    else {
        if (customerActiveID == null) {
            swal({
                title: "Bạn chắc chắn xóa chứ?",
                text: "",
                icon: "warning",
                buttons: true,
                dangerMode: true,
            }).then((isConFirm) => {
                if (isConFirm) {
                    $.ajax({
                        url: '/Agent/DeleteAgent',
                        data: { ID: id },
                        type: "POST",
                        success: function (response) {
                            if (response == SUCCESS) {
                                swal({
                                    title: "Xóa thành công!",
                                    text: "",
                                    icon: "success"
                                })
                                searchAgent();
                            } else {
                                swal({
                                    title: "Không thể xóa!",
                                    text: "Có lỗi.",
                                    icon: "warning"
                                })
                            }
                        },
                        error: function (result) {
                            console.log(result.responseText);
                        }
                    });
                }
            })
        } else {
            if (customerActiveID != null) {
                swal({
                    title: "Bạn chắc chắn muốn hủy kích hoạt đại lý này chứ?",
                    text: "",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true,
                }).then((isConFirm) => {
                    if (isConFirm) {
                        $.ajax({
                            url: '/Agent/cancelActive',
                            data: { ID: id },
                            type: "POST",
                            success: function (response) {
                                if (response == SUCCESS) {
                                    swal({
                                        title: "Hủy kích hoạt đại lý thành công!",
                                        text: "",
                                        icon: "success"
                                    })
                                    searchAgent();
                                } else {
                                    swal({
                                        title: "Không thể hủy kích hoạt đại lý!",
                                        text: "Có lỗi.",
                                        icon: "warning"
                                    })
                                }
                            },
                            error: function (result) {
                                console.log(result.responseText);
                            }
                        });
                    }
                })
            }
        }

    }
}

// hủy kích hoạt đại lý

function cancelActiveAgent(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    else {
        swal({
            title: "Bạn chắc chắn muốn hủy kích hoạt đại lý này chứ!",
            text: "",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Agent/cancelActive',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Hủy Kích Hoạt Thành Công",
                                text: "",
                                icon: "success"
                            });
                            $("#mdEdit").modal("hide");
                            setTimeout(function () {
                                searchAgent();
                            }, 1000);
                        } else {
                            swal({
                                title: "Có lỗi!",
                                text: "Vui lòng phản hồi với bộ phận hỗ trợ.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result, status, err) {
                        console.log(result.responseText);
                        console.log(status.responseText);
                        console.log(err.Message);
                    }
                });
            }
        })
    }
}

function SearchShop() {
    var NameShop = $('#NameShop').val().trim();
    var Province = $('#ProvinceID').val().trim();
    var district = $('#slDistrictShop').val();
    $.ajax({
        url: '/Shop/Search',
        data: {
            Page: 1,
            ShopName: NameShop,
            ProvinceID: Province,
            DistrictID: district,
        },
        success: function (response) {
            $('#TableShop').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}


function saveCreateShop() {
    var name = $.trim($('#Name').val());
    var contactName = $.trim($('#ContactName').val());
    var contactPhone = $.trim($('#ContactPhone').val());
    var province = $('#createShop #ProvinceID').val();
    var district = $('#slDistrictShopCreate').val();
    var place = $('#place').val();
    var long = $('#Long').val();
    var lat = $('#Lati').val();
    var address = $.trim($('#Address').val());
    var url = $(".imgCreateShop").attr("src");

    if (name == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập tên cửa hàng!",
            icon: "warning"
        })
        return;
    }
    else if (contactName == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập tên người liên hệ!",
            icon: "warning"
        })
        return;
    }
    else if (province < 1 || district < 1) {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn Tỉnh/Thành Quận/Huyện!",
            icon: "warning"
        })
        return;
    }
    else if (address == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập tên địa chỉ!",
            icon: "warning"
        })
        return;
    }
    else if (!isNumeric(contactPhone)) {
        swal({
            title: "Thông báo",
            text: "Số điện thoại chỉ được nhập số!",
            icon: "warning"
        })
        return;
    }
    else if (url == null) {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn ít nhất 1 ảnh!",
            icon: "warning"
        })
        return;
    }
    else if (place == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập vào địa chỉ URL!",
            icon: "warning"
        })
        return;
    }
    if (long == "" || lat == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn đúng và xác nhận Url maps!",
            icon: "warning"
        })
        return;
    }
    else {
        $.ajax({
            url: "/Shop/CreateShop",
            data: $('#form_create_shops').serialize(),
            success: function (response) {
                if (response == EXISTING) {
                    swal("Không thể tạo cửa hàng", "Vị trí bạn chọn đã được sử dụng cho 1 cửa hàng khác.", "warning");
                }
                else if (response == SUCCESS) {
                    swal("Thành công!", "", "success");
                    $('#createShop').modal('hide');
                    SearchShop();
                    resetInputShop();
                }
                else {
                    swal("Có lỗi xảy ra!", "Không thể tạo cửa hàng.", "warning");
                }
            }
        });
    }
}

//Sửa cửa hàng
function saveEditShop() {
    var name = $.trim($('#_Name').val());
    var contactName = $.trim($('#_ContactName').val());
    var contactPhone = $.trim($('#_ContactPhone').val());
    var address = $.trim($('#_Address').val());
    var url = $("._lstImage").attr("src");
    var province = $('#_ProvinceID').val();
    var district = $('#slDistrictShopUpdate').val();
    var place = $('#_Place').val();
    var long = $('#_Long').val();
    var lat = $('#_Lati').val();

    if (name == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập tên cửa hàng!",
            icon: "warning"
        })
        return;
    }
    else if (contactName == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập tên người liên hệ!",
            icon: "warning"
        })
        return;
    }
    else if (province < 1 || district < 1) {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn Tỉnh/Thành Quận/Huyện!",
            icon: "warning"
        })
        return;
    }
    else if (address == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập tên địa chỉ!",
            icon: "warning"
        })
        return;
    }
    else if (!isNumeric(contactPhone)) {
        swal({
            title: "Thông báo",
            text: "Số điện thoại chỉ được nhập số!",
            icon: "warning"
        })
        return;
    }
    else if (url == null) {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn ít nhất 1 ảnh!",
            icon: "warning"
        })
        return;
    }
    else if (place == "") {
        swal({
            title: "Thông báo",
            text: "Mời nhập vào địa chỉ URL!",
            icon: "warning"
        })
        return;
    }
    if (long == "" || lat == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng xác nhận Url maps!",
            icon: "warning"
        })
        return;
    }
    else {
        $.ajax({
            url: '/Shop/EditShop',
            type: 'POST',
            data: $('#form_edit_shop').serialize(),
            success: function (response) {
                if (response == SUCCESS) {
                    swal("Sửa thành công!", "", "success");
                    $('#EditShop').modal('hide');
                    SearchShop();
                    resetInputShop();
                } else
                    if (response == EXISTING) {
                        swal("Không thể sửa cửa hàng", "Vị trí bạn chọn đã được sử dụng cho 1 cửa hàng khác.", "warning");
                    }
                    else {
                        swal("Có lỗi xảy ra!", "Không thể sửa cửa hàng.", "warning");
                    }
            }
        });
    }
}



//Load Place to input
function LoadPlaceEditShop() {
    if ($('#_Place').val() != "") {
        var longlat = /\/\@(.*),(.*),/.exec($('#_Place').val());
        lat = longlat[1];
        lng = longlat[2];
        $('#_Lati').val(lat);
        $('#_Long').val(lng);
    }
    else {
        swal("Thông báo!", "Mời nhập vào địa chỉ UrL", "warning");
    }
}

//Load Place to input
function LoadPlaceCreateShop() {
    if ($('#place').val() != "") {
        var longlat = /\/\@(.*),(.*),/.exec($('#place').val());
        if (longlat == null) {
            swal("Thông báo", "Vui lòng chọn đúng đường dẫn google map", "warning");
        }
        lat = longlat[1];
        lng = longlat[2];
        if (lat == "" || lng == "") {
            swal("Thông báo", "Vui lòng chọn đúng đường dẫn google map", "warning");
        }
        $('#Lati').val(lat);
        $('#Long').val(lng);
    }
    else {
        swal("Thông báo!", "Mời nhập vào địa chỉ UrL", "warning");
    }
}

//reset text to default
function resetInputShop() {
    $('#Name').val('');
    $('#ContactName').val('');
    $('#ContactPhone').val('');
    $('#Address').val();
    $('#place').val('');
    $('#Lati').val('');
    $('#Long').val('');
    $('.Imgs').remove();
}

//reset itemSale 
function resetInputItemSale() {
    window.location = "/ItemSale/AddItemSale";
}
//reser text 
function resetInputItem() {
    $('#CodeCreate').val("");
    $('#NameCreate').val("");
    $('#PriceCreate').val();
    $('#DivtagImg').remove();
    window.location = "/Item/AddItem";
}
function resetBanner() {
    debugger
    $('#titleCreate').val("");
    $('#slTypeSend').val();
    $('#slType').val();
    window.location = "/Banner/AddBanner";
}

function resetVoucher() {
    //debugger
    //$('#titleCreate').val("");
    //$('#slTypeSend').val();
    //$('#slType').val();
    window.location = "/VoucherCustomer/AddVoucherCustomer";
}
function resetVoucherIntroduce() {
    //debugger
    //$('#titleCreate').val("");
    //$('#slTypeSend').val();
    //$('#slType').val();
    window.location = "/VoucherIntroduce/AddVoucherIntroduce";
}

//Lấy ra list url của shop cần sửa
function listUrlImage() {
    var url = "";
    $('._lstImage').each(function () {
        if (url == "") {
            url = $(this).attr('src');
        }
        else {
            url = url + "," + $(this).attr('src');
        }
    });
    $('#_txturlImage').val(url);
}

//Load modal edit shop
function loadModalEditShop($id) {
    var id = $id;
    $.ajax({
        url: '/Shop/loadModalEditShop',
        data: { ID: $id },
        success: function (response) {
            $('#modalEditShop').html(response);
            $('#EditShop').modal('show');
            listUrlImage();
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

//Delete Shop
function DeleteShop($id) {
    var id = $id
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Shop/DeleteShop',
                    data: { ID: id },
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Thông báo",
                                text: "Xóa thành công!",
                                icon: "success"
                            });
                            SearchShop();
                        }
                        else {
                            swal({
                                title: "Thông báo",
                                text: "Lỗi!",
                                icon: "warning"
                            });
                        }
                    }
                });
            }
        })
}

// Import Data Agent
function ImportAgent() {
    var fileUpload = $('#txtFile').get(0);
    var files = fileUpload.files;
    var formData = new FormData();
    formData.append('ExcelFile', files[0]);

    $.ajax({
        type: 'POST',
        url: '/Agent/ImportData',
        contentType: false,
        processData: false,
        data: formData,
        success: function (result) {
            if (result == 1) {
                swal({
                    title: "Import Thành Công",
                    text: "",
                    icon: "success"
                });
                searchAgent();
            }
            else if (result == -1) {
                swal({
                    title: "Hãy Chọn Một File Excel !",
                    text: "Có lỗi.",
                    icon: "warning"
                });
            }
            else if (result == 0) {
                swal({
                    title: "Mã Đại Lý Đã Tồn Tại !",
                    text: "Có lỗi.",
                    icon: "warning"
                });
            }
            else if (result == 3) {
                swal({
                    title: "File import không có dữ liệu",
                    text: "vui lòng kiểm tra lại !",
                    icon: "warning"
                });
            }
            else {
                swal({
                    title: "Sai Định Dạng File !",
                    text: "Có lỗi.",
                    icon: "warning"
                });
            }
            setTimeout(function () {
                $('#mdImport').modal('hide');
            }, 1000);
        }
    })
}


// Import Data Card
function importCard() {
    var fileUpload = $('#inputExcelFile').get(0);
    var files = fileUpload.files;
    var formData = new FormData();
    formData.append('ExcelFile', files[0]);

    $.ajax({
        type: 'POST',
        url: '/Card/Import',
        contentType: false,
        processData: false,
        data: formData,

        success: function (result) {
            if (result == 1) {
                swal({
                    title: "Import Thành Công",
                    text: "",
                    icon: "success"
                });
                searchCard();
            }
            else if (result == -1) {
                swal({
                    title: "Hãy Chọn Một File Excel !",
                    text: "Có lỗi.",
                    icon: "warning"
                });
            }
            else if (result == 0) {
                swal({
                    title: "Seri hoặc Mã thẻ Đã Tồn Tại !",
                    text: "Vui lòng kiểm tra lại",
                    icon: "warning"
                });
            }
            else if (result == -3) {
                swal({
                    title: "File Import Chưa Có Dữ Liệu !",
                    text: "Vui lòng kiểm tra lại",
                    icon: "warning"
                });
            }
            else if (result == -4) {
                swal({
                    title: "Lỗi !",
                    text: "Vui lòng liên vệ với bộ phận hỗ trợ",
                    icon: "warning"
                });
            }
            else if (result == -5) {
                swal({
                    title: "Mã thẻ hoặc seri phải hơn 10 kí tự",
                    text: "Vui lòng kiểm tra lại dữ liệu",
                    icon: "warning"
                });
            }
            else if (result > 20000 && result < 25000) {
                swal({
                    title: "Vui Lòng Điền Đầy Đủ Thông Tin",
                    text: "kiểm tra lại dòng " + (result - 20000),
                    icon: "warning"
                });
            }
            else if (result > 25000 && result < 30000) {
                swal({
                    title: "Seri hoặc mã thẻ được tối đa 15 ký tự",
                    text: "kiểm tra lại dòng " + (result - 25000),
                    icon: "warning"
                });
            }
            else if (result > 30000) {
                swal({
                    title: "Seri trùng với mã thẻ",
                    text: "kiểm tra lại dòng " + (result - 30000),
                    icon: "warning"
                });
            }
            else if (result > 1 && result <= 10000) {
                swal({
                    title: "Seri hoặc Mã thẻ Đã Tồn Tại !",
                    text: "vui lòng kiểm tra lại dòng " + result,
                    icon: "warning"
                });
            }
            else if (result > 10000 && result <= 20000) {
                swal({
                    title: "Dữ Liệu Không Hợp Lệ",
                    text: "vui lòng kiểm tra dòng " + (result - 10000),
                    icon: "warning"
                });
            }
            else {
                swal({
                    title: "Sai Định Dạng File !",
                    text: "Có lỗi.",
                    icon: "warning"
                });
            }
            setTimeout(function () {
                $('#mdImport').modal('hide');
            }, 1000);
        }
    });

}

// Back customer
function backToIndexCustomer() {
    $.ajax({
        type: "POST",
        url: "/Customer/GoHome",
        dataType: 'json',
        crossDomain: true,
        success: function (data) {
            window.location.href = data;
        }
    });
}

//Đổi thành khách hàng thân thiết
function changeRole(id) {
    $.ajax({
        type: "POST",
        url: "/Customer/ChangeRole",
        data: { ID: id },
        success: function (rs) {
            swal({
                title: "Thông báo",
                text: "Khách hàng bình thường",
                icon: "success"
            });
            GetCustomerDetail(id);
        }
    });
}
//Đổi thành khách hàng bình thường
function changeRoleBT(id) {
    $.ajax({
        type: "POST",
        url: "/Customer/ChangeRoleBT",
        data: { ID: id },
        success: function (rs) {
            swal({
                title: "Thông báo",
                text: "Khách hàng thân thiết",
                icon: "success"
            });
            GetCustomerDetail(id);
        }
    });
}

//function hideModelBatchDetail() {
//    $('.modal').hide();
//    $('#mdBatchDetail').hide();
//    $('.modal-backdrop').hide();
//}


//Format money in textbox
function cms_encode_currency_format(num) {
    if (!isNumeric(num)) {
        return '';
    }
    return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
}

function cms_decode_currency_format(obs) {
    if (obs == '')
        return '';
    else
        return parseInt(obs.replace(/,/g, ''));
}
function LoadRank($ID) {
    $.ajax({
        url: '/Config/ModalEditRank',
        data: { ID: $ID },
        type: 'POST',
        success: function (response) {
            $("#_listRank").html(response);
            $('#ModalEditRank').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
function LoadContactInfo(key) {
    $.ajax({
        url: '/Config/loadContactInfo',
        data: { keycheck: key },
        type: 'POST',
        success: function (response) {
            $("#_listRank").html(response);
            $('#ModalEditContactInfo').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
// modal Sửa danh mục
function editCategory($ID) {
    $.ajax({
        url: '/Category/ModalEditCategory',
        data: { ID: $ID },
        type: 'POST',
        success: function (response) {
            $("#UpdateCategory").html(response);
            $('#ModalEditCategory').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
// Edit Category
function SaveEditCategory(id) {
    var name = $.trim($('#txtEditName').val());
    var status = $('#slStatusEdit').val();
    var description = $.trim($("#contentEdit").val());
    var url = $(".imgEditCategory").val();
    index = url.lastIndexOf('files/');
    url = url.slice(index + 6);
    if (name == "") {
        swal({
            title: "Thông báo",
            text: "Mời bạn nhập vao tên danh mục",
            icon: "warning"
        });
        return;
    }
    else if (url == null) {
        swal({
            title: "Thông báo",
            text: "Mời bạn chọn ảnh",
            icon: "warning"
        });
        return;
    }
    else {
        $.ajax({
            url: '/Category/EditCategory',
            //data: $('#form_update_category').serialize(),
            data: {
                ID: id,
                Name: name,
                Status: status,
                Description: description,
                ImageUrl: url,
            },
            type: "POST",
            success: function (response) {
                if (response == SUCCESS) {
                    swal("Sửa danh mục thành công", "", "success");
                    SearchCategory();
                    //$("#ModalEditCategory").hide();
                    $("#ModalEditCategory").modal('toggle');
                }
                else {
                    swal("có lỗi xảy ra", "không thể sửa danh mục", "warning");
                }
            }
        });
    }

}
function SaveEditRank(id) {
    var maxPoint = $('#txtMaxPoint').val();
    var discountPercent = $('#txtDiscountPercent').val();
    var descriptions = $.trim($("#descriptions").val());
    if (maxPoint == "" || discountPercent == "") {
        swal({
            title: "Thông báo",
            text: "Mời bạn nhập đủ thông tin",
            icon: "warning"
        });
        return;
    }
    else {
        $.ajax({
            url: '/Config/SaveEditRank',
            data: {
                ID: id,
                MaxPoint: maxPoint,
                DiscountPercent: discountPercent,
                Descriptions: descriptions,
            },
            type: "POST",
            success: function (response) {
                if (response == SUCCESS) {
                    swal("Sửa thành công", "", "success");
                    $("#_listRank").html(response);
                    $('#ModalEditRank').modal('toggle');
                    location.reload();
                } else if (response == 419) {
                    swal("Sửa đổi thất bại", "Vui lòng kiểm tra lại mức điểm quy định", "warning");
                } else if (response == 420) {
                    swal("Sửa đổi thất bại", "Vui lòng kiểm tra lại mức giảm", "warning");
                } else if (response == 423) {
                    swal("Sửa đổi thất bại", "Mức giảm không được lớn hơn 100%", "warning");
                }
                else {
                    swal("có lỗi xảy ra", "không thể sửa hạng", "warning");
                }
            }
        });
    }

}
function SaveEditContactInfo(key) {
    var txtTextContactInfoZalo = $("#txtTextContactInfoZalo").val();
    var txtTextDescriptionContactInfoZalo = $.trim($("#txtTextDescriptionContactInfoZalo").val());
    var txtTextContactInfoMess = $("#txtTextContactInfoMess").val();
    var txtTextDescriptionContactInfoMess = $.trim($("#txtTextDescriptionContactInfoMess").val());
    var txtTextContactInfoHotline = $("#txtTextContactInfoHotline").val();
    var txtTextDescriptionContactInfoHotline = $.trim($("#txtTextDescriptionContactInfoHotline").val());
    $.ajax({
        url: '/Config/SaveEditContactInfo',
        data: {
            key: key,
            TextContactInfoZalo: txtTextContactInfoZalo,
            TextDescriptionContactInfoZalo: txtTextDescriptionContactInfoZalo,
            TextContactInfoMess: txtTextContactInfoMess,
            TextDescriptionContactInfoMess: txtTextDescriptionContactInfoMess,
            TextContactInfoHotline: txtTextContactInfoHotline,
            TextDescriptionContactInfoHotline: txtTextDescriptionContactInfoHotline,
        },
        type: "POST",
        success: function (response) {
            if (response == SUCCESS) {
                swal("Sửa đổi thành công", "", "success");
                $("#_listRank").html(response);
                $('#ModalEditContactInfo').modal('toggle');
                location.reload();
            }
            else {
                swal("có lỗi xảy ra", "không thể sửa", "warning");
            }
        }
    });


}

// Tim kiếm Danh mục
function SearchCategory() {
    var name = $('#txtName').val().trim();
    var status = $('#ctgr').val();
    var fromDate = $('#txtFromDate').val().trim();
    var category = $('#ctgr').val().trim();
    var toDate = $('#txtToDate').val().trim();
    $.ajax({
        url: "/Category/Search",
        data: {
            Page: 1,
            CateName: name,
            Status: status,
            FromDate: fromDate,
            ToDate: toDate,
            Category: category
        },
        type: "POST",
        success: function (response) {
            $('#TableCategory').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

// Tạo danh mục sản phẩm
function saveCreateCategory() {
    //var code = $.trim($('#code').val());
    var name = $.trim($('#name').val());
    var content = $.trim($('#content').val());
    var url = $('#AddImgLogoPlace').attr('src');
    index = url.lastIndexOf('files/');
    url = url.slice(index + 6);
    if (name == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập vào tên loại sơn",
            icon: "warning"
        })
        return;
    }
    else if (url == null) {
        swal({
            title: "Thông báo",
            text: "Vui lòng chọn ít nhất 1 ảnh",
            icon: "warning"
        })
        return;
    }
    else {
        $.ajax({
            url: "/Category/CreateCategory",
            data: {
                //Code: code,
                Name: name,
                Description: content,
                ImageUrl: url,
            },
            success: function (response) {
                debugger
                if (response == EXISTING) {
                    swal("Không thể tạo danh mục sản phẩm", "Danh mục đã tồn tại!!!", "warning")
                }
                else if (response == SUCCESS) {
                    swal("Tạo danh mục thành công", "", "success");
                    SearchCategory();
                    window.location = "/Category/Index";
                    $("#myModal").modal('toggle');
                    //$(".modal-backdrop").hide();
                }
                else {
                    swal("có lỗi xảy ra", "không thể tạo danh mục", "warning");
                }
            }
        });
    }
}
function DeleteCategory($ID) {
    swal({
        title: "Bạn chắc chắn xóa chứ?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Category/DeleteCategory',
                    data: { ID: $ID },
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Thông báo",
                                text: "Xóa thành công!",
                                icon: "success"
                            });
                            SearchCategory();
                        }
                        else {
                            if (response == 0) {
                                swal({
                                    title: "Thông báo",
                                    text: "Lỗi!",
                                    icon: "warning"
                                });
                            } else {
                                if (response == 3) {
                                    swal({
                                        title: "Không thể xóa danh mục cha khi danh mục cha đã tồn tại danh mục con của nó!",
                                        icon: "warning"
                                    });
                                    return;
                                } else {
                                    if (response == 4) {
                                        swal({
                                            title: "Không thể xóa danh mục vì tồn tại sản phẩm trong danh mục!",
                                            icon: "warning"
                                        });
                                        return;
                                    }
                                }
                            }

                        }
                    }
                });
            }
        });
}

//load dữ liệu lô hàng

function LoadBatch(BatchID) {
    $.ajax({
        url: "/Batch/ModalEditBatch",
        data: { BatchID: BatchID },
        type: 'POST',
        success: function (response) {
            $("#updateBatch").html(response);
            $('#myModal').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

//Lưu thông tin sửa lô hàng
function saveEditBatch() {
    var batchName = $("#batchName").val().trim();
    var point = $("#pointEdit").val().trim();
    var note = $("#noteEdit").val().trim();
    var price = $("#priceEdit").val().trim();
    var qty = $("#QtyStr").val();
    if (batchName == "" || point == "" || price == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin",
            icon: "warning"
        })
        return;
    }
    else {
        $.ajax({
            url: "/Batch/SaveEditBatch",
            data: $("#EditBatch").serialize(),
            type: "POST",
            success: function (response) {
                if (response == SUCCESS) {
                    swal({
                        title: "Thành công!",
                        text: "",
                        icon: "success"
                    });
                    $("#myModal").modal('hide');
                    searchBatch();
                } else {
                    if (response == 2) {
                        swal({
                            title: "Có lỗi xảy ra!",
                            text: "Không thể sửa số lượng khi lô hàng đã có sản phẩm được kích hoạt bảo hành",
                            icon: "warning"
                        });
                    } else {
                        swal({
                            title: "Có lỗi xảy ra!",
                            text: "Không thể sửa lô hàng",
                            icon: "warning"
                        });
                    }
                }
            },
            error: function (result) {
                console.log(result.responseText);
            }
        });
    }

}
function closeUserDetail() {
    window.location = '/Home/Index';
}
// start update user
function editUserDetal(id) {
    debugger
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({

    });
    var phone = $.trim($("#PhoneEdit").val());
    var userName = $.trim($("#UserEdit").val());
    var dob = $("#DOBEdit").val();
    var address = $.trim($("#AddressEdit").val());
    var sex = $("#slSexEdit").val();
    var email = $.trim($("#mailEdit").val());
    var role = $("#slRoleEdit").val();
    var status = $("#slStatusEdit").val();
    var provinceUDetail = $("#slProvinceUDetail").val();
    //var password = $.trim($("#PasswordEdit").val());
    var note = $.trim($("#NoteEdit").val());
    var url = $('#AddImgLogoPlace1').attr('src') || $('#AddImgLogoPlace').attr('src');
    if (userName.length == "" || phone.length == "" || dob.length == ""
        || role.length == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    } else
        if (!isNumeric(phone)) {
            swal({
                title: "Thông báo",
                text: "Số điện thoại chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        } else {
            $.ajax({
                url: '/User/UpdateUser',
                data: {
                    ID: id,
                    Phone: phone,
                    UserName: userName,
                    DOB: dob,
                    Address: address,
                    Sex: sex,
                    Email: email,
                    Role: role,
                    Status: status,
                    Province: provinceUDetail,
                    Note: note,
                    AvatarUrl: url,
                },
                type: 'POST',
                success: function (response) {
                    if (response.Status == 1 && response.Data.Role == 1) {
                        $('#EditUser').modal('hide');
                        swal({
                            title: "Sửa tài khoản thành công!",
                            text: "",
                            icon: "success"
                        })
                        window.location = '/User/Index';
                        searchUser();
                    } else if (response.Status == 1 && response.Data.Role != 1) {
                        swal({
                            title: "Sửa tài khoản thành công!",
                            text: "",
                            icon: "success"
                        })
                        window.location = '/Home/Index';
                    }
                    else {
                        swal({
                            title: "Sửa tài khoản không thành công!",
                            text: "",
                            icon: "warning"
                        })
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
}//end update user
// start update user
function editUser(id) {
    debugger
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({

    });
    var phone = $.trim($("#PhoneEdit").val());
    var userName = $.trim($("#UserEdit").val());
    var dob = $("#DOBEdit").val();
    var address = $.trim($("#AddressEdit").val());
    var sex = $("#slSexEdit").val();
    var email = $.trim($("#mailEdit").val());
    var role = $("#slRoleEdit").val();
    var status = $("#slStatusEdit").val();
    var provinceUserDetail = $("#slProvinceUserDetail").val();
    //var password = $.trim($("#PasswordEdit").val());
    var note = $.trim($("#NoteEdit").val());
    var url = $('#AddImgLogoPlace1').attr('src') || $('#AddImgLogoPlace').attr('src');
    if (userName.length == "" || phone.length == "" || dob.length == ""
        || role.length == "") {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    } else
        if (!isNumeric(phone)) {
            swal({
                title: "Thông báo",
                text: "Số điện thoại chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        } else {
            $.ajax({
                url: '/User/UpdateUser',
                data: {
                    ID: id,
                    Phone: phone,
                    UserName: userName,
                    DOB: dob,
                    Address: address,
                    Sex: sex,
                    Email: email,
                    Role: role,
                    Status: status,
                    Province: provinceUserDetail,
                    Note: note,
                    AvatarUrl: url,
                },
                type: 'POST',
                success: function (response) {
                    if (response.Status == 1) {
                        $('#EditUser').modal('hide');
                        swal({
                            title: "Sửa tài khoản thành công!",
                            text: "",
                            icon: "success"
                        })
                        window.location = '/User/Index';
                        searchUser();
                    } else {
                        swal({
                            title: "Sửa tài khoản không thành công!",
                            text: "",
                            icon: "warning"
                        })
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
}//end update user

//refresh user
function refreshUser(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn đưa tài khoản về mặc định?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willRefresh) => {
            if (willRefresh) {
                $.ajax({
                    url: '/User/RefreshUser',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Thành công!",
                                text: "",
                                icon: "success"
                            })
                            window.location = '/User/Index';
                            searchUser();
                        } else {
                            swal({
                                title: "Có lỗi xảy ra!",
                                text: "",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}

// xuất Excel List_Order
function exportListOrderExcel() {
    var fromDate = $("#txtFromDate").val();
    var toDate = $("#txtToDate").val();
    var status = $("#slStatus").val();
    var CusPhone = $("#txtCusPhone").val();
    window.location.href = "/Order/ExportOrder?FromDate=" + fromDate + "&ToDate=" + toDate + "&Status=" + status + "&Phone=" + CusPhone;
}
// xuất Excel One_Order
function exportOneOrderExcel(ID) {
    window.location.href = "/Order/ExportOneOrder?ID=" + ID ;
}
function RefreshCus(id) {

    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn đưa tài khoản về mặc định?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willRefresh) => {
            if (willRefresh) {
                $.ajax({
                    url: "/Customer/RefreshCustomer",
                    data: { ID: id },
                    success: function (res) {
                        if (res == SUCCESS) {
                            swal({
                                title: "Reset mật khẩu thành công !",
                                text: "",
                                icon: "success"
                            });
                        } else
                            swal({
                                title: "Có lỗi xảy ra!",
                                text: "Reset mật khẩu thất bại",
                                icon: "warning"
                            });
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}