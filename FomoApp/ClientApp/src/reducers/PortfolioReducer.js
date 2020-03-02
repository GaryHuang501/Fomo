import { RECIEVE_PORTFOLIOS } from '../actions/PortfolioActions';

export function portfolioReducer(state, action) {

    if (state === undefined) {
        return {
            portfolios: []
        };
    }

    switch (action.type) {
        case RECIEVE_PORTFOLIOS:
            return {
                portfolios: [...action.portfolios]
            };      
        default:
            return state;
    }
}

