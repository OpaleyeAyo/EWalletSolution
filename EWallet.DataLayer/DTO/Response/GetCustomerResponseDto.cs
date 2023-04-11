using System;
using System.Collections.Generic;

namespace EWallet.DataLayer.DTO.Response
{
    public class GetCustomerResponseDto
    {
        public Guid Id { get; set; }

        public Guid IdentityId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public IEnumerable<GetWalletResponseDto> Wallets { get; set; }
    }
}
