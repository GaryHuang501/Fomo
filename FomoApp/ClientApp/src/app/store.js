import chatReducer from '../features/chatbox/ChatSlice';
import { configureStore } from '@reduxjs/toolkit';
import leaderBoardReducer from '../features/leaderboard/LeaderBoardSlice';
import loginReducer from '../features/login/LoginSlice';
import membersReducer from '../features/members/MembersSlice';
import modalReducer from './modal/ModalSlice';
import portfolioReducer from '../features/portfolio/PortfolioSlice';
import stockSearchReducer from '../features/stockSearch/StockSearchSlice';
import stocksReducer from '../features/stocks/stocksSlice'

export const reducers = {
  reducer: {
    chat: chatReducer,
    leaderBoard: leaderBoardReducer,
    login: loginReducer,
    members: membersReducer,
    modal: modalReducer,
    portfolio: portfolioReducer,
    stocks: stocksReducer,
    stockSearch: stockSearchReducer,
  },
};

export const store = configureStore(reducers);
