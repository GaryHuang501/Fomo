import { RECIEVE_PORTFOLIOS, receivePortfolios } from '../actions/PortfolioActions';
import { portfolioReducer } from '../reducers/PortfolioReducer';

test("should return initial portfolio state as empty when state is undefined", () => {
    const state = portfolioReducer(undefined, RECIEVE_PORTFOLIOS);
    expect(state).not.toBeNull();
    expect(Array.isArray(state.portfolios)).toBeTruthy();
    expect(state.portfolios.length).toBe(0);
});

test("should return same state when action type is unknown", () => {
    const oldState = { someobject: "yes" };
    const state = portfolioReducer(oldState, "no idea what i am");
    expect(state).toBe(oldState);
});

test("should receive a new list portfolios when RECIEVE_PORTFOLIOS action", () => {
    const initialState = { portfolios: [] };
    const portfolios = [{ id: 1 }];
    const userId = 123;
    const receivePortfoliosAction = receivePortfolios(userId, portfolios);
    const state = portfolioReducer(initialState, receivePortfoliosAction);
    expect(state).toEqual(
        {
            portfolios: portfolios
        });
    expect(state.portfolios).not.toBe(portfolios);

});

test("should receive a new list portfolios with same values each time when RECIEVE_PORTFOLIOS action", () => {
    const initialState = { portfolios: [] };
    const portfolios = [{ id: 1 }];
    const userId = 123;
    const receivePortfoliosAction = receivePortfolios(userId, portfolios);
    const state1 = portfolioReducer(initialState, receivePortfoliosAction);
    const state2 = portfolioReducer(initialState, receivePortfoliosAction);
    expect(state1).toEqual(state2);
});
