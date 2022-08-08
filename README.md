# **Fomo App Overview**

![ ](https://github.com/GaryHuang501/Fomo/blob/master/FomoAPI/Docs/Fomo%20Architecure%201.drawio.png)


Fomo App is a stock portfolio sharing website. The main goal of this app was to learn newer technologies. The client app uses React and the server is .NET Web API with SQL Server. Firebase Real Time database is also used for real time messaging and pushing notifications to the clients.

The app uses asynchronous programming and eventual consistency. During market hours, users will be frantically requesting stock data in real time. Many times they will be requesting the same data. Thus, pulling the latest data every time from third-party APIs would be redundant, not very scalable, and expensive due to request limits. For a portfolio website, most users want realish time data, where exactly up to the minute is not required, which allows us to slowly push out updates.

![ ](https://github.com/GaryHuang501/Fomo/blob/master/FomoAPI/Docs/Asynchronous%20Stock%20Query%20Flow.drawio.png)


When a user adds a stock to their portfolio, they are automatically subscribed to the next update for that stock. Their stock’s initial value will be the current value in the cache or database. The application will periodically check a queue for stale stock data requests and prioritize by popularity. Once the stock request is executed, the database and cache is updated. The client will then be notified and request the latest stock data from the cache through the web api.

In addition, a schedule is run periodically to update any data for stocks that have been added to portfolios.

![ ](https://github.com/GaryHuang501/Fomo/blob/master/FomoAPI/Docs/Domain.drawio.png)


Domain Driven Design is important. For the most part, Wall Street terminology is universal and can be reused in our domain. But some APIs may have slightly different terminology and data structures for certain objects. So it’s important to have all external data conform to a standard domain and interface to be used through out the app. This allows us to easily swap between 3rd party providers without changing the code much and to have common understanding throughout our app.


