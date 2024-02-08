# User Documentation for Airlines website

## Introduction
Welcome to our Airlines Flight Booking Website.
This user guide will help you navigate our website with ease, ensuring a smooth experience as you find and book your next flight.

## Home Page
### Navigation Bar
At the top of the page, you will find the navigation bar with the `Theme` switch, `Home`, `Booking`, and `Summary` buttons. \
The navigation bar may collapse into a hamburger menu on smaller screens for a cleaner interface and better user experience.

### Collapsed Navigation Bar
On smaller devices, the navigation bar collapses to provide more screen space. To access the menu, click on the hamburger icon. \
The `Theme` switch can be accessed at the top right corner at all times.

### Dark Mode
The website supports a dark mode feature for easier viewing in low-light conditions. This can be activated by pressing the `Theme switch` located in the `Navigation Bar`.

## Booking Page
### Booking Process
Here, you can enter your flight details:
- `Origin City`: Enter your departure city.
- `Destination City`: Enter your arrival city, `Origin City` must be selected.
- `Adult count`: Input the number of adults, must be greater than `0`.
- `Children count`: Input the number of children, must be greater than `0`.
- `Tickets`: After entering your travel details, a list of available flights along with prices will be displayed. The flights might contain up to `2` transfers.
- `Total cost`: In the top right corner of every ticket there will be the cost of all transfers and the final price of the ticket.
- `Buy button`: In order to book a ticket the number of passengers must be greater than `0`. After pressing the button and the booking was a success a green alert will appear which contains the confirmation and a link which will redirect to the `Summary Page` or it will desappear after `5` seconds.

## Summary Page
This section gives you a summary of your purchased ticket details:

- `Flight Information`: Review the date, origin city and, destination city of your flight.
- `Closed Summary`: If the summary section is collapsed, you can expand it with a button on the right side of the flight to view all details by clicking on it.
- `Opened Summary`: If the summary section is opened, you can review the the details of your flight, including the transfers. Each transfer will include its price,
  - `Airline`
  - `Origin` and `Destination`,
  - `Distance`
  - `Price/km`,
  - `Passenger count`
  - `Price`

At the bottom you will see
  - `Total Distance`
  - `Total Flight Time`
  - `Total Price`

