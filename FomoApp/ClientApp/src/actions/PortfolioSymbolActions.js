import { sendFetch } from '../services/ApiClient';

export const ADD_PORTFOLIO_SYMBOL = 'ADD_PORTFOLIO_SYMBOL';
export const FETCH_PORTFOLIO_SYMBOLS = 'FETCH_PORTFOLIO_SYMBOLS';

export function receivePortfolioSymbols(portfolioId, symbols) {
    return {
        type: FETCH_PORTFOLIO_SYMBOLS,
        portfolioId: portfolioId,
        symbols: symbols
    };
}

export function addPortfolioSymbolPost(portfolioId, exchange, symbolName) {
    return {
        type: ADD_PORTFOLIO_SYMBOL,
        portfolioId: portfolioId,
        exchange: exchange,
        symbolName: symbolName
    };
}

export function fetchPortfolioSymbols(portfolioId) {

    return function (dispatch) {
        return Promise.resolve(
            [
                { exchange: "NYSE", symbolName: "TESLA" },
                { exchange: "NYSE", symbolName: "GOOG" }
            ]
        )
        .then(function (symbols) {
            dispatch(receivePortfolioSymbols(portfolioId, symbols));
        })
        .catch(function () {

        });
    };
}

export function addPortfolioSymbol(portfolioId, exchange, symbolName) {

    return function (dispatch) {
        dispatch(addPortfolioSymbolPost(portfolioId, exchange, symbolName));
    };
}
