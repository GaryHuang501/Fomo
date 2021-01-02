import { configureStore } from '@reduxjs/toolkit';
import loginReducer from '../features/login/LoginSlice';
import portfolioReducer from '../features/portfolio/PortfolioSlice';
import stockSearchReducer from '../features/stockSearch/StockSearchSlice';
import stocksReducer from '../features/stocks/stocksSlice'

export default configureStore({
  reducer: {
    login: loginReducer,
    stockSearch: stockSearchReducer,
    portfolio: portfolioReducer,
    stocks: stocksReducer
  },
});
