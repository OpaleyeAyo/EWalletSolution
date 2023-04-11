
namespace EWallet.DataLayer.DTO.Generic
{
    public static class AsyncEventType
    {
        public const string CreateWalletsForCustomer = "CREATE_USER_WALLET";
        public const string CreateNewSettlementAccount = "CREATE_SETTLEMENT_ACCOUNT";
        public const string SendInterestAdditionNotification = "SIMPLE_INTEREST_ADDITION_NOTIFICATION";
        public const string Undertermined = "UNDETERMINED";
    }
}
