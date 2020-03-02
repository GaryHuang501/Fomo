export const requestPortfolios = jest.fn((userId) => { });
export const receivePortfolios = jest.fn((userId, portfolios) => { });
export const fetchPortfolios = jest.fn((userId) => { });

export default { requestPortfolios, receivePortfolios, fetchPortfolios}