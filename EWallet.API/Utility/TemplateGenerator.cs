using EWallet.DataLayer.Contracts;
using System.Text;

namespace EWallet.API.Utility
{
    public class TemplateGenerator
    {
        private readonly ITransactionRepository _transactionRepository;
        
        public TemplateGenerator(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public string GetHtmlString()
        {
            var transactions = _transactionRepository.GetAll();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(
                @"
                    <html>
                        <head></head>
                        <body>
                            <div class='header'><h2>List of Transactions</h2></div>
                            <table align = 'center'>
                                <tr>
                                    <th>Id</th>
                                    <th>TransactionReference</th>
                                    <th>TransactionSourceAccount</th>
                                    <th>CurrencyCode</th>
                                    <th>TransactionDestinationAccount</th>
                                    <th>Amount</th>
                                    <th>TransactionDate</th>
                                    <th>TypeOfTransaction</th>
                                    <th>TransactionStatus</th>
                                </tr>
                ");

            foreach (var transaction in transactions)
            {
                stringBuilder.AppendFormat(
                    @"
                        <tr>
                            <td>{0}</td>
                            <td>{1}</td>
                            <td>{2}</td>
                            <td>{3}</td>
                            <td>{4}</td>
                            <td>{5}</td>
                            <td>{6}</td>
                            <td>{7}</td>
                            <td>{8}</td>
                        </tr>",
                    transaction.Id,
                    transaction.TransactionReference,
                    transaction.TransactionSourceAccount,
                    transaction.CurrencyCode,
                    transaction.TransactionDestinationAccount,
                    transaction.Amount,
                    transaction.TransactionDate,
                    transaction.TypeOfTransaction,
                    transaction.TransactionStatus);
            }

            stringBuilder.Append(@"
                                    </table>
                                </body>
                            </html>");

            return stringBuilder.ToString();
        }
    }
}
