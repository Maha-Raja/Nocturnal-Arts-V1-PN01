using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Common.Constants
{
    public static class Constants
    {
        public const string GENERAL_ERROR_MESSAGE = "An error has occured. Please see logs for details.";
        public const string PASSWORD_CHANGE_ERROR = "Unable to change the Password. Please try again Later";
        public const string GENERAL_SUCCESS_MESSAGE = "Success";
        public const int NO_DAYS_FOR_SLOTS = 29;

        public const string VENUE_IMAGE_POST_EVENT_LKP_HIDDEN = "7";
        public const string INVALID_REQUEST_BODY = "Invalid Request Body";
        public const string INVALID_OLD_PASSWORD = "Invalid Old Password";
        public const string INVALID_NEW_PASSWORD = "New Password cannot be same as Old Password";
        public const string LOGIN_SUCCESS_MESSAGE = "Login Successfull";
        public const string LOGIN_ERROR_MESSAGE = "Invalid Username or Password";
        public const string LOGIN_USER_TYPE_ERROR_MESSAGE = "You do not have access to perform this operation.";
        public const string LOGIN_UNVERIFIED_EMAIL_MESSAGE = "Verify your email to continue";
        public const string LOGIN_UNVERIFIED_PHONE_MESSAGE = "Verify your phone number to continue";
        public const string FORBIDDEN_ERROR_MESSAGE = "User cannot validate";
        public const string UNAUTHORISED_ERROR_MESSAGE = "User unauthorized";
        public const string EVENT_STATUS_LOOKUP = "EVENT_STATUS_TYPE";
        public const string EVENT_STATUS_SCHEDULED = "1";
        public const string EVENT_STATUS_PENDING = "2";
        public const string EVENT_STATUS_CANCELLED = "3";
        public const string EVENT_STATUS_TYPE = "EVENT_STATUS_TYPE";
        public const string ARTIST_PAYMENT_METHOD = "ARTIST_PAYMENT_METHOD";
        public const string ARTIST_PAYMENT_METHOD_CREDIT = "CREDIT";
        public const string ARTIST_PAYMENT_METHOD_CARD = "CARD";
        public const string THRESHOLD_OF_TICKETS_FOR_ORDER_CREATION = "ThresholdOfTicketsForOrderCreation";
        public const string EVENT_IMAGES_LOOKUP = "EVENT_IMAGES";
        public const string TICKET_STATUS_CANCELLED = "CANCELLED";
        public const string ARTIST_MANAGER_ROLE_CODE = "ARTIST_MANAGER";
        public const string ARTIST_USER_REFERENCE_TYPE = "artist";

        /// <summary>
        /// ORder Constants
        /// </summary>
        public const string DAYS_TO_CREATE_ORDER_BEFORE = "DAYS_TO_CREATE_ORDER_BEFORE";
        public const string DAYS_TO_MOVE_ORDER_TO_BOOKED_STATUS = "DAYS_TO_MOVE_ORDER_TO_BOOKED_STATUS";
        public const string SHIP_ORDER_BEFORE_IN_DAYS = "SHIP_ORDER_BEFORE_IN_DAYS";
        public static Dictionary<string, string> UserReferenceType = new Dictionary<string, string>() {
            { "ADMIN", "admin" },
            { "ARTIST", "artist" },
            { "VCP", "vcp" },
            { "CUSTOMER", "customer" },
        };
        public static Dictionary<string, string> ReferenceType = new Dictionary<string, string>() {
            { "ARTIST", "artist" },
            { "VENUE", "venue" },
        };
        public static Dictionary<string, string> PaintingStatus = new Dictionary<string, string>() {
            { "Rejected", "R" },
            { "Approved", "A" },
            { "Pending", "P" },
        };
        public static Dictionary<string, string> UserRoleCode = new Dictionary<string, string>() {
            { "ADMIN", "ADMIN" },
            { "ARTIST", "ARTIST" },
            { "VCP", "VCP" },
            { "CUSTOMER", "CUSTOMER" },
        };
        public static Dictionary<string, string> FollowStatus = new Dictionary<string, string>() {
            { "FOLLOW", "follow" },
            { "UNFOLLOW", "unfollow" },
        };
        public static Dictionary<string, string> LikeStatus = new Dictionary<string, string>() {
            { "LIKE", "LIKE" },
            { "UNLIKE", "UNLIKE" },
        };
        public static Dictionary<string, int> MarkeId = new Dictionary<string, int>() {
            { "A", 1 },
            { "B", 2 },
            { "C", 3 },
            { "D", 4 },
            { "E", 5 },
            { "F", 6 },
            { "G", 7 },
            { "H", 8 },
            { "I", 9 },
            { "J", 10 },
            { "K", 11 },
            { "L", 12 },
            { "M", 13 },
            { "N", 14 },
            { "O", 15 },
            { "P", 16 },
            { "Q", 17 },
            { "R", 18 },
            { "S", 19 },
            { "T", 20 },
            { "U", 21},
            { "V", 22},
            { "W", 23},
            { "X", 24},
            { "Y", 25},
            { "Z", 26},
        };
    }
}
