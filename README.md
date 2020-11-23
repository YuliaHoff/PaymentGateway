# Checkout.com - Payment Gateway

## Description

- PaymentGateway solution contains the PaymentGateway project that is the web API of the required specifications.
- AcquiringBankSimulator solution contains the bank simulator to test the API of PaymentGateway

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
