import { combineReducers } from 'redux';
import { portfolioReducer } from './PortfolioReducer';
import { portfolioSymbolReducer } from './PortfolioSymbolReducer';
import { loginReducer } from './LoginReducer';

const rootReducer = combineReducers({
    portfolioReducer,
    portfolioSymbolReducer,
    loginReducer
});

export default rootReducer;
