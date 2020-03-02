jest.mock('../actions/PortfolioActions');
import { mapStateToProps, mapDispatchToProps } from './PortfolioContainer';
import PortfolioActionModule from '../actions/PortfolioActions';


test("Should map portfolio correctly to props", () => {
    var state = {
        portfolioReducer: { portfolios: [] }
    };
    var mappedState = mapStateToProps(state);
    expect(mappedState).toEqual({ portfolios: [] });
});

test("Should map fetchPortfolios() to props onLoad", () => {
    var dispatch = (action) => { };
    var mappedDispatch = mapDispatchToProps(dispatch);
    mappedDispatch.onLoad();
    expect(PortfolioActionModule.fetchPortfolios.mock.calls.length).toBe(1);

});