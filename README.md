# Checkout.com - Payment Gateway

## Description

- PaymentGateway solution contains the PaymentGateway project that is the web API of the required specifications.
- AcquiringBankSimulator solution contains the bank simulator to test the API of PaymentGateway

## How to run

1. Install .NET 5 runtime
2. Run PaymentGateway on IIS / IIS express
3. Run AcquiringBankSimulator on IIS / IIS express (notice that on IIS the bank simulator path will not be the same as in the code, so I recommend IIS express for simplicity)
4. Authenticate to the payment gateway using the api (POST): https://localhost:44333/api/users/Authenticate 
body should be raw JSON:
```
{
    "UserName" : "Apple",
    "Password" : "Apple"
}
```
5. Copy the token received from the request
6. To add a new payment, make a request to the api (POST): https://localhost:44333/api/Payments
Headers should contain an Authorization key with the value copied in the previous step (for example: `asdf - 12/1/2020 1:52:12 PM - 35088628`)
Body should be raw JSON:
```
{ 
    "Amount" : 6800,
    "CardCVV" : 553,
    "CreditCardNumber": "3432432432463463",
    "ExpirationDate": "2029-02-15T00:00:00Z",
    "Currency": "Euro"
}
```

## Swagger API documentation

- When running the server (PaymentGateway), API documentation will be available on: [https://{serverurl}/swagger/ui/index.html](https://localhost:44333/swagger/ui/index.html)

## Development Notes

### Authentication details

- User must authenticate to access the APIs
- The user passwords are hashed in the code. 
- To authenticate use: 
`{
    "UserName" : "Amazon",
    "Password" : "Amazon"
}`
- Currently the database is in memory and initialized with hardcoded temp data
- Calling the authenticate endpoint will result in a token that must be used in every subsequent request in the "Autorization" header

### TODO list - things that would have been done if I had more time

- Add many many more tests: 
  - Test other functions of PaymentService and test other services
  - Test other functions of PaymentController and test UserController
- Connect to real database and replace the InMemory one that is used
- Move to a config file:
  - Log file path
  - Bank API address
- Add more error handlings and more write to log 
- Improve authorization token
- Add user registration endpoint
- Add user input validation (such as: currency is an actual currency)

### Assumptions

- Masking is done on a 16 chars credit number where first 6 and last 4 chars are not masked
- The acquiring bank does not require authentication and identifies just by the user id (this assumption is only for simplicity since I don't know the authentication model the bank will eventually use)

### Other notes

- Accidentally read the spec as "retrieve details of a previously made payments" instead of "retrieve details of a previously made payment". Decided to leave it since I have already done it when I realized I actually only need to get 1. Also, it makes sense the merchant might want to get all of his payments to analyze his data.

## Testing

PaymentGateway solution contains 2 project: one is the API and the other tests the first project
