using System;
using System.Threading;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public interface ILevi9PaymentService
    {
        Task<PaymentResponse> MakePayment();
    }

    // DO NOT TOUCH
    public class Levi9PaymentService : ILevi9PaymentService
    {
        public async Task<PaymentResponse> MakePayment()
        {
            Random random = new Random();

            // Simulate long proccess
            Thread.Sleep(3000);

            // 20% chances for unhappy flow
            var randomNumber = random.Next(0, 9);
            if (randomNumber == 1)
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    Message = "Connection error."
                };
            }

            if (randomNumber == 2)
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    Message = "Insufficient founds."
                };
            }

            return new PaymentResponse
            {
                IsSuccess = true,
                Message = "Payment is successful."
            };
        }
    }
}
