import { sendFetch } from '../services/ApiClient';

export const REQUEST_PORTFOLIOS = 'REQUEST_PORTFOLIOS';
export const RECIEVE_PORTFOLIOS = 'RECIEVE_PORTFOLIOS';
export const SWITCH_PORTFOLIO = 'SWITCH_PORTFOLIO';

export function requestPortfolios(userId) {
    return {
        type: REQUEST_PORTFOLIOS,
        userId: userId
    };
}

export function receivePortfolios(userId, portfolios) {
    return {
        type: RECIEVE_PORTFOLIOS,
        userId: userId,
        portfolios: portfolios
    };
}

export function switchPortfolio(portfolioId) {
    return {
        type: SWITCH_PORTFOLIO,
        selectedPortfolioId: portfolioId
    };
}

export function fetchPortfolios(userId) {

    return function (dispatch) {
        dispatch(requestPortfolios(userId));
        //return sendFetch("https://localhost:44395/api/values", dispatch)
        return Promise.resolve(
                {
                    portfolios: [
                        {
                            portfolioId: "1",
                            name: "Portfolio Test"
                        },
                        {
                            portfolioId: "2",
                            name: "Portfolio Test2"
                        }
                    ]
                }
            )
            .then(function (portfolios) {
                dispatch(receivePortfolios(userId, portfolios));
            })
            .catch(function () {

            });
  
    };
}