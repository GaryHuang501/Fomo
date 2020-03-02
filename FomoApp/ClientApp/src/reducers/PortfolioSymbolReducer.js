import {FETCH_PORTFOLIO_SYMBOLS, ADD_PORTFOLIO_SYMBOL } from '../actions/PortfolioSymbolActions';


function createSymbol({ exchange, symbolName }) {
    return {
        exchange,
        symbolName
    };
}
export function portfolioSymbolReducer(state, action) {

    if (state === undefined) {
        return {
            portfoliosToSymbols: {}
        };
    }

    switch (action.type) {
        case FETCH_PORTFOLIO_SYMBOLS:
            return {
                portfoliosToSymbols: {
                    [action.portfolioId]: action.symbols
                }
            };
        case ADD_PORTFOLIO_SYMBOL:
         
            var newSymbolListForPortfolio = [...state.portfoliosToSymbols[action.portfolioId], createSymbol(action)];
            return {
                portfoliosToSymbols: Object.assign(
                    {},
                    state.portfoliosToSymbols,
                    { [action.portfolioId]: newSymbolListForPortfolio }
                )
            };
        default:
            return state;
    }
}

