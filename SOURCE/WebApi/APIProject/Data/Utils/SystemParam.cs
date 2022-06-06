using Data.Business;
using Data.DB;
using Data.Model.APIWeb;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Utils
{
    public class SystemParam : GenericBusiness
    {
        //public const string PASS_DEFAULT = "daiichivn";
        //public const string HOST_DEFAULT = "smtp.gmail.com";
        //public const string EMAIL_CONFIG = "daiichisuport@gmail.com";
        //public const string PASS_CONFIG = "qqkbewqyqudknent";
        //public const string PASS_EMAIL = "windsoft123456@";

        public const string PASS_DEFAULT = "chophongthuy";
        public const string HOST_DEFAULT = "smtp.gmail.com";
        public const string EMAIL_CONFIG = "mrfishhotro@gmail.com";
        public const string PASS_CONFIG = "qwrlumhuqrotgoez";
        public const string PASS_EMAIL = "123456Aa@";
        //public const string EMAIL_CONFIG = "chophongthuy123@gmail.com";
        //public const string PASS_CONFIG = "qwrlumhuqrotgoez";
        //public const string PASS_EMAIL = "ChoPhongThuy@123";
        //public const string URL_WEB_SOCKET = "http://localhost:3001/";
        public const string URL_WEB_SOCKET = "http://apidev.tpmart.winds.vn/api/v1/notification/push-socket";
        //public const string URL_WEB_SOCKET = "http://gpt.winds.vn:8092/socketio";
        //public const string URL_WEB_SOCKET = "http://mrfish.winds.vn/socket.io";
        //public const string PASS_CONFIG = "sqsusteaeaztongt";
        //public const string PASS_EMAIL = "windsoft123456a@";
        // Config One Signal   
        // Thông báo
        public const string NOTI_MR_FISH = "MR_FISH thông báo";
        public const string URL_ONESIGNAL = "https://onesignal.com/api/v1/notifications";
        public const string URL_BASE_https = "Basic ://onesignal.com/api/v1/notifications";
        public const string APP_ID = "d392c6af-b3cd-4856-9a92-52333b38e9cb";
        public const string Authorization = "Basic :ZDU5YzM4MjctNmE1OS00MTE3LWFkODQtYWNlZWJjZmEyMjAz";
        public const string ANDROID_CHANNEL_ID = "834b9681-3d22-4ed0-ad73-9e57dcc02aea";

        public const int TIME_DELAY = 30000;

        // VNPAY KEY
        public const string vnp_CodeSucces = "00";
        public const string vnp_Url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public const string vnp_TmnCode = "HCXWXOEJ";
        public const string vnp_HashSecret = "TUIJCVNAQJFOHFOCKOUBOSJEDMFASXQW";
        public const string vnp_Return_url = "http://mrfish.winds.vn/VnPay/Index";

        public const string TRANSACTION_SUCCESS = "Giao dịch thành công";
        public const string TRANSACTION_FAIL = "Giao dịch thất bại";
        public const string Transaction_FAIL_CODE = "Giao dịch thất bại mã lỗi : ";

        public const string customer_success = "mrfish://success/";
        public const string customer_failed = "mrfish://failed/";

        //Các type thông báo và noti
        public const int NOTIFY_NAVIGATE_ORDER = 1; //xác nhận, hủy, thanh toán đơn hàng => báo về app, app => màn hình chi tiết đơn hàng
        public const int NOTIFY_NAVIGATE_NEWS = 2;//thông báo về bài viết mới => vào màn hình chi tiết bài viết
        public const int NOTIFY_NAVIGATE_LIKE = 3;//thông báo về like bài viết hoặc comment
        public const int NOTIFY_NAVIGATE_MESSAGE = 4; // thông báo về có tin nhắn mới từ khách hàng của admin

        public const int NOTI_VIEWED = 1;
        public const int NOTI_NOT_VIEWED = 0;


        public const int TIME_REMOVE_ORDER_NOT_PAID = 10;
        public const int CONFIG_TIME = 5;
        public const int ROLL_ADMIN = 1;
        public const int ROLL_CUSTOMER = 0;
        public const int POINT_START = 10;
        public const int POINT_RANKING_START = 0;
        public const int QTY_CONTENT_HOME_SCREEN = 10;
        public const int LIMIT_DEFAULT = 10;
        public const int TYPE_LOGIN_FACE = 1;
        public const int TYPE_LOGIN_GOOGLE = 2;
        public const int TYPE_LOGIN_PHONE = 3;

        

        //nữ
        public const int MALE = 0;
        //nam
        public const int FEMALE = 1;

        public const int QTY_DEFAULT_ADD_TO_CART = 1;
        public const int GET_ALL_PRODUCT = 0;

        public const int GENDER_MEN = 0;
        public const int GENDER_WOMEN = 1;

        public const int PROVINCE_DEFAULT = 1;
        public const int DISTRICT_DEFAULT = 1;

        public const int NO_NEED_UPDATE = 0;
        public const int NEED_UPDATE = 1;

        public const int Ward_DEFAULT = 1;

        public const int RECEIVE_ADDRESS_DEFAULT = 1;
        public const int RECEIVE_ADDRESS_NOT_DEFAULT = 0;

        public const string CONVERT_DATETIME = "dd/MM/yyyy";
        public const string CONVERT_DATETIME1 = "yyyy-MM-dd";
        public const string CONVERT_DATETIME_HAVE_HOUR = "dd/MM/yyyy HH:mm";
        public const int MAX_ROW_IN_LIST = 30;
        public const int LIMIT_ORDER = 20;
        public const int ACTIVE = 1;
        public const int RETURN_TRUE = 1;
        public const int RETURN_FALSE = 0;
        public const int ACTIVE_FALSE = 0;
        public const int NO_ACTIVE = 2;
        public const int COUNT_NULL = 0;
        public const int DELETE_REQUEST_FAIL = 2;
        public const int CATEGORY_PRODUCT = 11;

        // Type Notification
        public const int NOTIFICATION_TYPE_ORDER = 1; // Navigate đến màn chi tiết đơn hàng
        public const int NOTIFICATION_TYPE_NEWS = 2; // Navigate đến màn chi tiết tin tức
        public const int NOTIFICATION_TYPE_COUPON = 3; // Navigate đến màn chi tiết mẫ giảm giá
        public const int NOTIFICATION_TYPE_POINT = 4; // Navigate đến màn điểm tích lũy
        public const int NOTIFICATION_TYPE_RANK = 5; // Navigate đến màn hạng

        //diem % config
        public const int POINT_CONFIG = 419;
        public const int PERCENT_CONFIG = 420;
        //Voucher status
        public const int VOUCHER_STATUS = 421;
        //CHECK %
        public const int CHECK_PERCENT = 423;
        //sửa sp
        public const int EDIT_ITEM = 417;
        //agent
        public const int TYPE_REQUEST_AGENT = 1;
        public const int STATUS_ACCEPT_REQUEST_AGENT = 1;
        public const int STATUS_CANCEL_REQUEST_AGENT = 0;
        public const int STATUS_PENDING_REQUEST_AGENT = 2;

        public const int EXIST_AGENT_CODE = 99;
        public const int EXIST_PHONE_AGENT = 98;
        public const string MESSAGE_EXIST_PHONE_AGENT = "Số điện thoại này đã được yêu cầu hoặc thuộc về 1 đại lý khác.";

        public const int TYPE_IMAGE = 1;
        public const int TYPE_VIDEO = 2;

        public const string TOKEN_INVALID = "Token invalid";
        public const string TOKEN_NOT_FOUND = "Token not found";
        public const string DEVICE_ID_NOT_FOUND_MESSAGE = "DeviceID not found";
        public const string SERVER_ERROR = "Hệ thống đang bảo trì";
        public const string MESSAGE_ERROR = "Thất bại";
        public const string ERROR_ADD_TO_CART_MESSAGE = "Không thể thêm sản phẩm vào giỏ hàng.";
        public const string MESSAGE_INVALID_NUMBER = "Số lượng không được phép âm";
        public const string MESSAGE_REMOVE_CART_NO_ID = "Vui lòng chọn sản phẩm để xóa khỏi giỏ hàng";
        public const string MESSAGE_CREATE_ORDER_NO_ID = "Vui lòng chọn sản phẩm để đặt hàng.";
        public const string MESSAGE_ERROR_USER_INFO = "Vui lòng nhập đầy đủ họ tên, địa chỉ";
        public const string MESSAGE_EXISTING_REQUEST_AGENT = "Đã tồn tại yêu cầu đăng ký. Vui lòng chờ hệ thống xác nhận";
        public const string MESSAGE_ERROR_UPDATE_PHONE = "Không thể thay đổi số điện thoại";
        public const string MESSAGE_ERROR_EXIST_PHONE = "Số điện thoại đã được sử dụng";
        public const string MESSAGE_ERROR_EXIST_EMAIL = "Email đã được sử dụng";
        public const string MESSAGE_CODE_COLOR_INVALID = "Vui lòng nhập đúng định dạng mã màu HEX";
        public const string MESSAGE_NOTIFY_NOT_FOUND = "Thông báo không tồn tại";
        public const string MESSAGE_EXIST_ACCOUNT = "Số điện thoại này đã được sử dụng.";
        public const string MESSAGE_ERROR_PHONE_NOT_FOUND = "Số điện thoại chưa được đăng ký";
        public const string MESSAGE_LOGIN_ACCOUNT_FAIL = "Sai tài khoản hoặc mật khẩu";
        public const string MESSAGE_ERROR_CREATE_ORDER_NO_DATA = "Vui lòng chọn sản phẩm để tạo đơn hàng";
        public const string MESSAGE_ERROR_UPDATE_CART_NO_DATA = "Vui lòng chọn sản phẩm để cập nhật";
        public const string MESSAGE_REQUIRE_REGISTER = "Vui lòng nhập họ tên, số điện thoại và mật khẩu";
        public const string MESSAGE_NOT_EXIST_PRODUCT = "Sản phẩm không tồn tại";
        public const string MESSAGE_NOT_EXIST_NEWS = "Bài viết không tồn tại";
        public const string MESSAGE_NOT_EXIST_COUPON = "Mã giảm giá không tồn tại";
        public const string MESSAGE_ADD_TO_CART_FAIL = "Sản phẩm không tồn tại để thêm vào giỏ hàng";
        public const string MESSAGE_ADD_TO_CART_SUCCESS = "Thêm sản phẩm vào giỏ hàng thành công";
        public const string MESSAGE_ERROR_REMOVE_CART_FAIL = "Không thể xóa sản phẩm khỏi giỏ hàng. Hệ thống không tìm thấy thông tin sản phẩm.";
        public const string MESSAGE_NOT_EXIST_INFO = "Hệ thống không tìm thấy thông tin.";
        public const string MESSAGE_LIST_EMPTY = "Danh sách dữ liệu hiện đang rỗng.";
        public const string MESSAGE_ERROR_INVALID_PHONE = "Vui lòng nhập đúng định dạng số điện thoại.";
        public const string MESSAGE_NOT_EXIST_USER = "Hệ thống không tìm thấy người dùng với số điện thoại này";
        public const string MESSAGE_CANT_GIVE_POINT_FOR_ME = "Bạn không thể tặng điểm cho chính mình";
        public const string MESSAGE_ERROR_GIVE_POINT_NO_POINT = "Vui lòng nhập số điểm cần tặng";
        public const string MESSAGE_KHONG_DU_DIEM_DE_TANG = "Số điểm khả dụng của bạn không đủ để tặng điểm. Vì số điểm tối thiểu trong tài khoản phải duy trì ở mức ";
        public const string MESSAGE_VUOT_QUA_HAN_MUC_QUY_DINH = "Bạn không thể tặng điểm vì đã dùng hết điểm trong 1 ngày. Số điểm có thể dùng trong 1 ngày bằng ";
        public const string MESSAGE_GIVE_POINT_SUCCESS = "Tặng điểm thành công";
        public const string MESSAGE_ERROR_WRONG_PASSWORD = "Mật khẩu cũ không đúng";
        public const string MESSAGE_ERROR_CHANGE_PASSWORD = "Đổi mật khẩu thất bại";
        public const string MESSAGE_ERROR_NEW_PASSWORD_SAME_OLD_PASSWORD = "Mật khẩu mới không được phép giống mật khẩu hiện tại";
        public const string MESSAGE_CHANGE_PASSWORD_SUCCESS = "Đổi mật khẩu thành công";
        public const string MESSAGE_ERROR_NOT_EXIST_AGENT_CODE = "Mã đại lý không tồn tại trong hệ thống";
        public const string MESSAGE_ERROR_AGENT_CODE_USED = "Mã đại lý đã được sử dụng";
        public const string MESSAGE_ACTIVE_AGENT_SUCCESS = "Tài khoản của bạn đã được kích hoạt thành đại lý";
        public const string MESSAGE_ERROR_CUSTOMER_WAS_AGENT = "Tài khoản của bạn đã là đại lý. Không thể thực hiện thao tác này";
        public const string MESSAGE_WARRANTY_CODE_USED = "Mã bảo hành đã được sử dụng";
        public const string MESSAGE_WARRANTY_CODE_NOT_EXIST = "Mã kích hoạt bảo hành không tồn tại";
        public const string MESSAGE_SCAN_QR_FAIL = "Mã đã được sử dụng hoặc không tồn tại";
        public const string MESSAGE_ACTIVE_WARRANTY_SUCCESS = "Kích hoạt bảo hành thành công";
        public const string MESSAGE_ERROR_INVALID_EMAIL = "Sai định dạng Email";
        public const string MESSAGE_NEW_PASSWORD_SENT = "Mật khẩu đã mới được gửi về email của bạn. Vui lòng kiểm tra email";
        public const string MESSAGE_OTP_SENT = "Mã OTP được gửi về email của bạn. Vui lòng kiểm tra email";
        public const string MESSAGE_ERROR_NOT_EXIST_EMAIL = "Email không tồn tại trong hệ thống";
        public const string MESSAGE_ERROR_NOT_EXIST_OTP = "Mã OTP không khả dụng";
        public const string MESSAGE_ERROR_INVALID_POINT = "Vui lòng nhập đúng định dạng số điểm";
        public const string MESSAGE_ERROR_DISTRICT_NOT_IN_PROVINCE = "Quận (huyện) hoặc phường (xã) bạn chọn không phù hợp với Tỉnh thành";
        public const string MESSAGE_ERROR_PROVINCEID_NULL = "Vui lòng chọn Tỉnh thành";
        public const int ERROR_NOT_EXIST_NEWS = 536;
        public const string MESSAGE_ERROR_NOT_EXIST_NEWS = "Bài viết không tồn tại";
        public const int ERROR_NOT_EXIT_COMMENT = 537;
        public const string MESSAGE_ERROR_NOT_EXIT_COMMENT = "Không tồn tại bình luận";
        public const int MESSAGE_ERROR_COMMENT_TYPE = 535;
        public const string MESSAGE_ERROR_COMMENT_NULL = "Không tồn tại nội dung bình luận";
        public const int MESSAGE_ERROR_DATA_TYPE = 538;
        public const string MESSAGE_ERROR_DATA = "Dữ liệu lỗi";
        public const int ERROR_LES_THAN_0 = 539;
        public const string MESSAGE_ERROR_LES_THAN_0 = "Dữ liệu không được phép âm";
        public const int ERROR_NOT_REPAIR = 540;
        public const string MESSAGE_ERROR_NOT_REPAIR = "Không được phép sửa";
        public const int ERROR_NOT_DATA = 541;
        public const string MESSAGE_ERROR_NOT_DATA = "Không tồn tại dữ liệu";

        public const int PERMISSION_ERROR_CODE = 542;
        public const string PERMISSION_ERROR_STR = "Tài khoản của bạn không có quyền thực hiện chức năng này";



        // thanh cong
        public const int SUCCESS_CODE = 200;
        public const string SUCCESS_MESSAGE = "Thành công";
        public const string SUCCESS_MESSAGE_UPDATE_CART = "Cập nhật giỏ hàng thành công";
        // sai mk
        public const int ERROR_PASS_API = 403;
        // loi quy trinh
        public const int PROCESS_ERROR = 500;
        public const int FAIL = 501;
        public const int ERROR_UPDATE_PHONE = 502;
        public const int ERROR_EXIST_EMAIL = 503;
        public const int ERROR_EXIST_PHONE = 505;
        public const int ERROR_ADD_TO_CART = 506;
        public const int CODE_INVALID_NUMBER = 507;
        public const int REMOVE_CART_NO_ID = 508;
        public const int CREATE_ORDER_NO_ID = 509;
        public const int UPDATE_CART_FAIL = 511;
        public const int ERROR_USER_INFO = 512;
        public const int ERROR_LOGIN_ACCOUNT_FAIL = 513;
        public const int ERROR_CREATE_ORDER_NO_DATA = 514;
        public const int ERROR_UPDATE_CART_NO_DATA = 515;
        public const int REQUIRE_REGISTER = 516;
        public const int NOT_EXIST_PRODUCT = 517;
        public const int ERROR_REMOVE_CART_FAIL = 518;
        public const int KHONG_DU_DIEM_DE_TANG = 519;
        public const int VUOT_QUA_HAN_MUC_QUY_DINH = 520;
        public const int ERROR_WRONG_PASSWORD = 521;
        public const int ERROR_NEW_PASSWORD_SAME_OLD_PASSWORD = 522;
        public const int ERROR_NOT_EXIST_AGENT_CODE = 523;
        public const int ERROR_AGENT_CODE_USED = 524;
        public const int ERROR_CUSTOMER_WAS_AGENT = 525;
        public const int ERROR_INVALID_EMAIL = 526;
        public const int ERROR_NOT_EXIST_EMAIL = 527;
        public const int ERROR_DISTRICT_NOT_IN_PROVINCE = 528;
        public const int ERROR_PROVINCEID_NULL = 529;
        public const int ERROR_PHONE_NOT_FOUND = 534;
        public const int ERROR_ITEM_NOT_FOUND = 502;
        public const string MESSAGE_ITEM_NOT_FOUND = "Sản phẩm không tồn tại";
        public const int ERROR_CART_UPDATED = 503;


        public const int ERROR_CART_EMPTY = 504;
        public const string MESSAGE_CART_EMPTY = "Giỏ hàng trống ";
        public const int ERROR_RECEIVE_ADDRESS_NOT_FOUND = 505;
        public const string MESSAGE_RECEIVE_ADDRESS_NOT_FOUND = "Thông tin người nhận không tồn tại";
        public const int ERROR_COUPON_NOT_VALID = 506;
        public const string MESSAGE_COUPON_NOT_VALID = "Mã giảm giá không hợp lệ";
        public const int ERROR_VNPAY_MONEY_EXCEED = 507;
        public const string MESSAGE_VNPAY_MONEY_EXCEED = "Số tiền thanh toán phải lớn hơn hoặc bằng 10000 đồng";
        public const string MESSAGE_ADD_RECEIVE_ADDRESS_FAIL= "Thêm thông tin người nhận thất bại";
        public const string MESSAGE_UPDATE_RECEIVE_ADDRESS_FAIL= "Cập nhật thông tin người nhận thất bại";
        public const string MESSAGE_CART_UPDATED = "Giỏ hàng đã được cập nhật ";
        public const string MESSAGE_ADD_CART_FAIL = "Thêm vào giỏ hàng thất bại ";
        public const string MESSAGE_UPDATE_CART_FAIL = "Cập nhật giỏ hàng thất bại ";
        public const string MESSAGE_CREATE_ORDER_FAIL = "Đặt hàng thất bại ";

        public const int ERROR_ORDER_CART_UPDATED = -1;
        public const int ERROR_ORDER_RECEIVE_ADDRESS_NOT_FOUND = -2;
        public const int ERROR_ORDER_CART_EMPTY = -3;
        public const int ERROR_ORDER_COUPON_NOT_VALID = -4;
        public const int ERROR_ORDER_NOT_FOUND = 502;
        public const int ERROR_ORDER_STATUS_UPDATED = 503;
        public const string MESSAGE_ORDER_NOT_FOUND = "Đơn hàng không tồn tại ";
        public const string MESSAGE_ORDER_STATUS_UPDATED = "Trạng thái đơn hàng đã bị thay đổi";


        public const int EXIST_ACCOUNT = -500;
        public const int EXIST_PHONE_REGISTER = -501;
        public const int EXIST_EMAIL_REGISTER = -502;
        public const int ADD_TO_CART_FAIL = -503;
        public const int ERROR_INVALID_PHONE = -504;
        public const int WARRANTY_CODE_USED = -505;
        public const int WARRANTY_CODE_NOT_EXIST = -506;
        public const int ERROR_INVALID_POINT = -507;



        public const int ERROR = 0;
        public const int SUCCESS = 1;
        public const int DUPLICATE_PHONE = 2;

        public const int NOT_FOUND = 404;
        public const int DATA_NOT_FOUND = 400;
        public const string DATA_NOT_FOUND_MESSAGE = "Kiểm tra dữ liệu đầu vào";
        public const int DEVICE_ID_NOT_FOUND = 300;
        // khong duoc phep
        public const int UNAUTHORIZED = 401;

        //đã tồn tại yêu cầu đăng ký đại lý
        public const int EXISTING_REQUEST_AGENT = 303;

        public const int STATUS_RUNNING = 1;
        public const int STATUS_REQUEST_WAITING = 0;
        public const int STATUS_REQUEST_SUCCESS = 1;
        public const int STATUS_REQUEST_CANCEl = 2;
        public const int STATUS_REQUEST_DELETE = 3;
        
        // Type đổi quà
        public const int TYPE_POINT_SAVE = 1;
        public const int TYPE_POINT_GIVE = 2;
        public const int TYPE_POINT_RECEIVE = 3;
        public const int TYPE_POINT_RECEIVE_GIFT = 4;
        public const int TYPE_ADD_POINT = 5;
        public const int TYPE_CARD = 6;

        public const string TYPE_POINT_SAVE_ICON = "https://image.flaticon.com/icons/png/512/2333/2333325.png";
        public const string TYPE_POINT_GIVE_ICON = "https://image.flaticon.com/icons/png/512/1643/1643098.png";
        public const string TYPE_POINT_RECEIVE_ICON = "https://image.flaticon.com/icons/png/512/1643/1643146.png";
        public const string TYPE_POINT_RECEIVE_GIFT_ICON = "https://image.flaticon.com/icons/png/512/2308/premium/2308818.png";
        public const string TYPE_ADD_POINT_ICON = "https://image.flaticon.com/icons/png/512/1415/premium/1415467.png";
        public const string TYPE_CARD_ICON = "https://image.flaticon.com/icons/png/512/1041/premium/1041555.png";


        public const int SIZE_CODE = 8;
        public const int LENGTH_AGENT_CODE = 8;
        public const int MIN_NUMBER = 100000;
        public const int MAX_NUMBER = 999999;

        // Status warranty 
        public const int W_STATUS_ACTIVE = 1;
        public const int W_STATUS_NO_ACTIVE = 2;
        public const int W_STATUS_ERROR = 3;

        // cách kiểu tích điểm
        public const int WARRANTY = 2;
        public const int PRODUCT = 1;

        //
        public const int MESS_BY_CUS = 1;
        public const int MESS_BY_ADMIN = 2;
        //
        public const int HISTORY_TYPE_ADD_ANOTHER = 3;
        public const int HISTORY_TYPE_ADD_PRODUCT = 1;
        public const int HISTORY_TYPE_ADD_WARRANTY = 2;
        //

        // Loại mã giảm giá
        public const int COUPON_TYPE_RANK = 1; // voucher khách hàng
        public const int COUPON_TYPE_REFER = 2; // voucher giới thiệu

        public const int COUPON_DISCOUNT_MONEY = 1; // voucher giảm theo tiền
        public const int COUPON_DISCOUNT_PERCENT = 2; // voucher giảm theo %


        // danh mục banner
        public const int NEWS_TYPE_BANNER = 1;
        public const string NEWS_TYPE_BANNER_STRING = "Banner";
        // danh mục chính sách
        public const int NEWS_TYPE_POLICY = 2;
        public const string NEWS_TYPE_POLICY_STRING = "Chính sách";

        //tysend
        public const int TYPESEND_BANNER_DRAFT = 0;
        public const string TYPESEND_BANNER_DRAFT_STRING = "Lưu nháp";
        // danh mục chính sách
        public const int TYPESEND_BANNER = 1;
        public const string TYPESEND_BANNER_STRING = "Đăng bài";


        public const int ITEM_HOT = 1;
        public const int ITEM_HOT_FALSE = 0;

        public const int ITEM_FLASH_SALE = 1;
        public const int ITEM_FLASH_SALE_FALSE = 0;

        public const int ORDER_PRICE_LOW_TO_HIGH = 1;
        public const int ORDER_PRICE_HIGH_TO_LOW = 0;

        public const string ITEM_ACTIVE = "Đang hoạt động";
        public const string ITEM_ACTIVE_FALSE = "Ngưng hoạt động";


        public const string COMMENT_HISTORY_ADD_POINT = "Tích điểm khuyễn mãi";
        public const string COMMENT_HISTORY_SAVE_POINT_PRODUCT = "Kích hoạt bảo hành sản phẩm";
        // link check access Token
        public const string LINK_URL_FACEBOOK = "https://graph.facebook.com/me?fields=name,picture.height(960).width(960)&access_token=";
        public const string LINK_URL_GOOGLE_MAIL = "https://www.googleapis.com/plus/v1/people/me?access_token=";
        public const string LINK_URL_GOOGLE_MAIL2 = "https://www.googleapis.com/plus/v2/people/me?access_token=";
        // Telecom
        public const int MAX_TELECOM = 4;
        public const string URL_VIETTEL = "https://upload.wikimedia.org/wikipedia/vi/thumb/e/e8/Logo_Viettel.svg/800px-Logo_Viettel.svg.png";
        public const string URL_MOBIPHONE = "https://upload.wikimedia.org/wikipedia/commons/d/de/Mobifone.png";
        public const string URL_VINAPHONE = "https://lozimom.com/wp-content/uploads/2017/04/vinaphone-logo.png";
        public const string URL_VIETNAMMOBILE = "https://upload.wikimedia.org/wikipedia/vi/thumb/a/a8/Vietnamobile_Logo.svg/1280px-Vietnamobile_Logo.svg.png";
        public const int TELECOM_TYPE_GIFT = 0;
        public const int TYPE_VIETTEL = 1;
        public const int TYPE_MOBIPHONE = 2;
        public const int TYPE_VINAPHONE = 3;
        public const int TYPE_VIETNAMMOBILE = 4;
        public const string TYPE_VIETTEL_STRING = "Viettel";
        public const string TYPE_MOBIPHONE_STRING = "Mobiphone";
        public const string TYPE_VINAPHONE_STRING = "Vinaphone";
        public const string TYPE_VIETNAMMOBILE_STRING = "VietnamMobile";
        public const string URL_FIRST = "https://graph.facebook.com/";
        public const string URL_LAST = "/picture?type=large&redirect=true&width=250&height=250";
        public const int STATUS_PRODUCT_ACTIVE = 1;
        public const int STATUS_PRODUCT_NO_ACTIVE = 2;
        public const string STATUS_PRODUCT_ACTIVE_STRING = "Đã sử dụng";
        public const string STATUS_PRODUCT_NO_ACTIVE_STRING = "Chưa sử dụng";

        public const int STATUS_REQUEST_PENDING = 0;
        public const int STATUS_REQUEST_ACCEPTED = 1;
        public const int STATUS_REQUEST_CANCEL = 2;
        public const string STATUS_REQUEST_PENDING_STRING = "Chờ xác nhận";
        public const string STATUS_REQUEST_ACCEPTED_STRING = "Đã xác nhận";
        public const string STATUS_REQUEST_CANCEL_STRING = "Hủy";

        public const int TYPE_REQUEST_GIFT = 1;
        public const int TYPE_REQUEST_VOUCHER = 2;
        public const int TYPE_REQUEST_CARD = 3;

        public const string TYPE_REQUEST_GIFT_STRING = "Quà tặng";
        public const string TYPE_REQUEST_VOUCHER_STRING = "Voucher";
        public const string TYPE_REQUEST_CARD_STRING = "Thẻ cào";

        public const int TYPE_GIFT_GIFT = 1;
        public const int TYPE_GIFT_VOUCHER = 2;
        public const int TYPE_GIFT_CARD = 3;

        public const int STATUS_GIFT_ACTIVE = 1;
        public const int STATUS_GIFT_PAUSE = 0;
        public const int STATUS_GIFT_CANCEL_AND_ADD = 2;
        public const int STATUS_GIFT_CANCEL = 3;

        public const int NO_ACTIVE_DELETE = 0;
        public const int MAX_ROW_IN_LIST_WEB = 20;
        public const bool BOOLEAN_TRUE = true;
        public const bool BOOLEAN_FALSE = false;
        public const int DUPLICATE_NAME = 2;

        public const int QRCODE_TYPE_PRODUCT = 1;
        public const int QRCODE_TYPE_WARRANTY = 2;

        public const int STATUS_CARD_ACTIVE = 1;
        public const int STATUS_CARD_NO_ACTIVE = 2;
        public const int ERROR_DATE = 3;


        //public const string App_ID = "6bb1c7ed-5ca7-4576-9fea-cd40523a2de8";
        //public const string Channel_ID = "a4509146-84c0-4deb-9c72-ca1397173a04";
        //public const string Rest_API_KEY = "M2M5NGNjZGYtNzYzZi00YTNiLWE5MTAtYjIyZDk0ZTkzYjg3";
        //public const string URL_Notification_POST = "https://onesignal.com/api/v1/notifications";

        public const string PHONGTHUY_NOTI = "Shop Giang Phong Thủy";

        public const string REQUIRE_FIELD = "Vui lòng không để trống!";
        public const string PUSH_NOTIFY_ONESIGNAL = "https://onesignal.com/api/v1/notifications";
        public const string INVALID_NUMBER = "Chỉ được phép nhập số!";
        public const string LOGIN_FAIL = "Vui lòng nhập đúng số điện thoại";
        public const float KeyA = 11;
        public const float KeyB = 87;
        public const float KeyC = 48;
        public const string ICON_GIFT = "http://icons.iconarchive.com/icons/lovuhemant/merry-christmas/256/Gift-icon.png";
        public const string ICON_MONEY = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT1TeKukaS-4k9SggMYEP4VpmsTMwVUlhDeTIOSiNyBbhrVATdv";
        public const string ICON_BONUS = "https://image.flaticon.com/icons/png/512/536/536089.png";
        public const string ICON_ORDER_CANCEL = "https://image.flaticon.com/icons/png/512/172/172120.png";
        public const string ICON_ORDER_CONFIRM = "https://image.flaticon.com/icons/png/512/262/262807.png";
        public const string ICON_ORDER_PAID = "https://image.flaticon.com/icons/png/512/172/172165.png";
        public const string ICON_NOTIFY_TYPE_POINT_GIVE = "https://image.flaticon.com/icons/png/512/1643/1643098.png";
        public const string ICON_NOTIFY_TYPE_POINT_RECEIVE = "https://image.flaticon.com/icons/png/512/1643/1643146.png";
        public const string ICON_NOTIFY_TYPE_CREATE_NEWS = "https://image.flaticon.com/icons/png/512/1598/1598991.png";
        public const string ICON_NOTIFY_TYPE_REQUEST = "https://image.flaticon.com/icons/png/512/2308/premium/2308818.png";
        public const string ICON_BULLHORN = "https://image.flaticon.com/icons/png/512/172/172173.png";

        public const int NOTIFY_TYPE_ORDER_CANCEL = 1;
        public const int NOTIFY_TYPE_ORDER_CONFIRM = 2;
        public const int NOTIFY_TYPE_ORDER_PAID = 3;
        public const int NOTIFY_TYPE_POINT_GIVE = 4;
        public const int NOTIFY_TYPE_POINT_RECEIVE = 5;
        public const int NOTIFY_TYPE_CREATE_NEWS = 6;
        public const int NOTIFY_TYPE_CANCEL_AGENT = 7;

        public const int TYPE_NO_SEND = 4;
        public const int TYPE_SEND_CUSTOMER = 1;
        public const int TYPE_SEND_CUSTOMER_LOYAL = 3;
        public const int TYPE_SEND_AGENCY = 2;
        public const int TYPE_SEND_ALL = 0;

        public const int SEND_NOTIFY = 1;

        public const int SHOW_HOME_SCREEN = 1;
        public const int NO_SHOW_HOME_SCREEN = 0;

        public const int STATUS_NEWS_ACTIVE = 1;
        public const int STATUS_NEWS_DRAFT = 0;
        public const int UPDATE_NEWS_DEFAULT = 1;
        public const int UPDATE_NEWS_POST = 0;
        public const int LENGTH_QR_HASH = 15;
        public const int EXISTING = 2;
        public const int EXIST_TIME_INPUT = 413;
        public const int EXIST_TIME = 414;
        public const int EXIST_COMPARE = 415;
        public const int EXIST_QUAMTITY = 416;
        public const int ERROR_CODE = 418;
        public const int CAN_NOT_DELETE = 2;
        public const int ROLE_USER_ORDER = 3;
        public const int ROLE_USER = 2;
        //Khách hàng thân thiết
        public const int ROLE_CUSTOMER_LOYAL = 2;
        //Khách hàng bình thường
        public const int ROLE_AGENT = 2;
        public const int NOT_ADMIN = 3;
        public const int WRONG_PASSWORD = 2;
        public const int FAIL_LOGIN = 2;
        public const int TYPE_REQUEST_NOTIFY = 4;
        //public const int TYPE_ORDER_NOTIFY = 7;

        //ROLE tài khoản
        public const int ROLE_ADMIN = 1;//Admin
        public const int ROLE_EDITOR = 2; //Biên tập
        public const int ROLE_ACCOUNTANT = 3; // Kế toán
        public const int ROLE_CUSTOMER = 4; //Customer
        //Rank
        public const int RANK_MEMBERSHIP = 1;//HẠNG THANH VIÊN
        public const int RANK_SILVER = 3; //hẠNG BẠC
        public const int RANK_GOLDEN = 4; // HẠNG VÀNG
        public const int RANK_DIAMOND = 5; //HẠNG KIM CƯƠNG
        //Giới tính
        public const int MEN = 0; //ĐÀN ÔNG
        public const int WOMEN = 1; //ĐÀN BÀ


        public const int MAX_PEOPLE = 90;

        public const string MAX_POINT_PER_DAY = "MaxPointPerDay";
        public const string MIN_POINT = "MinPoint";

        public const string KHONG_DU_DIEM_DOI_QUA_MESSAGE = "Số điểm khả dụng của bạn không đủ để đổi quà. Vì số điểm tối thiểu trong tài khoản phải duy trì ở mức ";
        public const string DA_DUNG_HET_SO_DIEM_TRONG_NGAY_MESSAGE = "Bạn không thể đổi quà vì đã dùng hết điểm trong 1 ngày. Số điểm có thể dùng trong 1 ngày bằng ";
        public const string GIFT_REQUEST_NOT_FOUND_MESSAGE = "Đổi quà thất bại. Hệ thống không tim thấy thông tin món quà bạn yêu cầu.";
        public const string CONFIRM_FAIL = "Hệ thống đã hết thẻ. Vui lòng chọn thẻ khác";
        public const string CREATE_REQUEST_SUCCESS_MESSAGE = "Yêu cầu đã được gửi thành công. Vui lòng chờ phản hồi từ hệ thống";
        public const string DIEM_DOI_QUA_LON_HON_DIEM_MINH_MESSAGE = "Số điểm của bạn không đủ để đổi quà";

        public const int KHONG_DU_DIEM_DOI_QUA = -201;
        public const int DA_DUNG_HET_SO_DIEM_TRONG_NGAY = -202;
        public const int GIFT_REQUEST_NOT_FOUND = -203;
        public const int DIEM_DOI_QUA_LON_HON_DIEM_MINH = -204;

        //Card
        public const string IMPORT_CARD_VIETTEL = "viettel";
        public const string IMPORT_CARD_MOBIPHONE = "mobi";
        public const string IMPORT_CARD_VINAPHONE = "vina";
        public const string IMPORT_CARD_VIETNAMOBILE = "vnmobile";
        public const int TELECOMTYPE_VIETTEL = 1;
        public const int TELECOMTYPE_MOBIPHONE = 2;
        public const int TELECOMTYPE_VINAPHONE = 3;
        public const int TELECOMTYPE_VIETNAMOBILE = 4;
        public const int ERROR_IMPORT_DUPLICATE = 0;

        // status, type trong bảng Orders
        public const int TYPE_CART = 1;
        public const int TYPE_ORDER = 2;

        public const int MIX_COLOR = 1;
        public const int NO_MIX_COLOR = 0;


        public const int STATUS_CART_PENDING = 0;
        public const int STATUS_CART_BOUGHT = 1;

        // status order 
        public const int STATUS_ORDER_REFUND = -2; //Hoàn tiền
        public const int STATUS_ORDER_CANCEL = -1; //Hủy
        public const int STATUS_ORDER_PENDING = 0; //Chờ xác nhận
        public const int STATUS_ORDER_PROCESS = 1; //Đang thực hiện
        public const int STATUS_ORDER_DONE = 2; // Hoàn thành
        public const string STATUS_ORDER_REFUND_MSG = "Hoàn tiền";
        public const string STATUS_ORDER_CANCEL_MSG = "Hủy"; 
        public const string STATUS_ORDER_PENDING_MSG = "Chờ xác nhận"; 
        public const string STATUS_ORDER_PROCESS_MSG = "Đang thực hiện"; 
        public const string STATUS_ORDER_DONE_MSG = "Hoàn thành"; 


        //public const string CONTACT_INFO
        // status order payment
        public const int ORDER_PAID = 1; // Đã thanh toán
        public const int ORDER_NOT_PAID = 0; // Chưa thanh toán

        public const int PAYMENT_TYPE_TRANSFER = 2; // Thanh toán chuyển khoản
        // type payment
        public const int PAYMENT_TYPE_ON_DELIVERY = 1; // Thanh toán tiền mặt
        public const int PAYMENT_TYPE_MOMO = 2; // Thanh toán MOMO
        public const int PAYMENT_TYPE_VNPAY = 3; // Thanh toán VNPAY

        // config
        public const string CONFIG_BONUS_POINT_PERCENT_KEY = "BonusPointPercent";
        public const double CONFIG_BONUS_POINT_PERCENT_VALUE_DEFAULT = 1;

        // Status for all
        public const int STATUS_ACTIVE = 1;
        public const int STATUS_NO_ACTIVE = 0;
         // IsHot Sản phẩm bán chạy
        public const int ISHOT_ACTIVE = 1;
        public const int UN_ISHOT_ACTIVE = 0;

        // Not Found
        public const int PHONE_NOT_FOUND = -1;

        // ExcelFile Error
        public const int FILE_NOT_FOUND = -1;
        public const int FILE_DATA_DUPLICATE = 0;
        public const int FILE_IMPORT_SUCCESS = 1;
        public const int FILE_FORMAT_ERROR = -2;
        public const int FILE_EMPTY = -3;
        public const int IMPORT_ERROR = -4;
        public const int MIN_LENGTH_VALIDATE = -5;
        public const int DATA_ERROR = -6;

        // check Length Card
        public const int MAX_LENGTH_CODE = -7;
        public const int MAX_LENGTH_SERI = -8;
        public const int CODE_EQUALS_SERI = -9;

        // type MemberPointHistory
        public const int HISPOINT_TICH_DIEM = 1;
        public const int HISPOINT_TANG_DIEM = 2;
        public const int HISPOINT_DUOC_TANG_DIEM = 3;
        public const int HISPOINT_DOI_QUA = 4;
        public const int HISPOINT_HE_THONG_CONG_DIEM = 5;
        public const int HISPOINT_DOI_THE = 6;
        public const int HISTORY_POINT_CANCEL_REQUEST = 7;


        //trạng thái đơn hàng
        public const int CANCEL_STATUS_ORDER = -1; //đã hủy
        public const int AGAIN_STATUS_ORDER = -2; //đã hoàn tiền
        public const int AWAIT_STATUS_ORDER = 0; //chờ xác nhận
        public const int PROCESSING_STATUS_ORDER = 1; //đang thực hiện
        public const int COMPLETED_STATUS_ORDER = 2; //hoàn thành

        //
        public const int ACTIVE_AGENT = 1;

        
        public const string SEND_TIME = "null";
        public const string USER = "dekko";
        public const string PASS = "Vmg@123456";
        public const string ALIAS = "DEKKO GROUP";
        public const string LINK_MESS = "http://brandsms.vn:8018/VMGAPI.asmx/BulkSendSms?";
        public const string CONTENT_MESS = "Cam on quy khach da dang ky. Ma xac nhan cua quy khach la :";

        //Nam ta 
        public const string MESSAGE_INSERT_SUCCESS = "Thêm mới dữ liệu thành công!";
        public const string MESSAGE_UPDATE_SUCCESS = "Cập nhật dữ liệu thành công!";

        // minh an
        public const int TYPE_NEWS_BANNER = 3;

        //config jwt
        public static string JWT_KEY = ConfigurationManager.AppSettings.Get("Jwt_key");
        public static string JWT_ISSUER = ConfigurationManager.AppSettings.Get("Jwt_issuer");
        public static int JWT_EXPIES = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Jwt_expires"));

        public static IJsonPacketFactory DSN_SENTRY { get; internal set; }


        //config
        public const string PERCENTAGE = "Percentage";
        public const string DESCRIPTIONCONTACTINFO_ZALO = "DescriptionContactInfoZalo";
        public const string CONTACTINFO_ZALO = "ContactInfoZalo";
        public const string DESCRIPTIONCONTACTINFO_MESS = "DescriptionContactInfoMess";
        public const string CONTACTINFO_MESS = "ContactInfoMess";
        public const string DESCRIPTIONCONTACTINFO_HOTLINE = "DescriptionContactInfoHotline";
        public const string CONTACTINFO_HOTLINE = "ContactInfoHotline";

        public const int ID_ZALO = 1;
        public const int ID_MESS = 2;
        public const int ID_HOTLINE = 3;

        public const string STR_ZALO = "Zalo";
        public const string STR_MESS = "Messager";
        public const string STR_HOTLINE = "Hotline";
    }
}
