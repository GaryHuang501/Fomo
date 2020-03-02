import * as portfolioActions from "./PortfolioActions";

test("Should create new request portfolio action", () => {
    const userId = 123;
    const action = portfolioActions.requestPortfolios(userId);

    expect(action.type).toBe(portfolioActions.REQUEST_PORTFOLIOS);
    expect(action.userId).toBe(userId);
});

test("Should create new receive portfolio action", () => {
    const userId = 123;
    const portfolios = [{ id: 1 }];
    const action = portfolioActions.receivePortfolios(userId, portfolios);
    expect(action.type).toBe(portfolioActions.RECIEVE_PORTFOLIOS);
    expect(action.userId).toBe(userId);
    expect(action.portfolios).toEqual(portfolios);
});
