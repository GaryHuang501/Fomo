#Fomo App Overview

Fomo App is a stock portfolio sharing website. The main goal of this app was to practice applying modern technologies. The front end uses React and the backend uses .NET Core and SQL Server. Firebase Real Time database is also used for both JSON data storage and for pushing messages or notifications to the clients.

The concept of asynchronous programming and eventually consistency is key to this app. During market hours, users will be frantically requesting stock data in real time. However, pulling the latest data every time from third-party APIs would be slow, redundant, financially expensive due to request limits, and not very scalable. For a portfolio website, most users want real time data, but not exactly up to the minute. So this allows us to slowly push out updates,

When a user adds a stock to their portfolio, they are automatically subscribed to the next update for that stock. Their stock’s initial value will be the current value in the cache or database. The application will periodically check a queue for stale stock data requests and prioritize by popularity. Once the stock request is executed, the database and cache is updated. The client will then be notified and request the latest stock data from the cache.

Domain Driven Design is another key principle used. For the most part, Wall Street terminology is universal and can be reused in our domain. But some APIs may have slightly different terminology and data structures for certain objects. So it’s important to have all external data conform to a standard domain and interface to be used through out the app. This allows us to easily swap between 3rd party providers without changing the code much and to have common understanding throughout our app.


