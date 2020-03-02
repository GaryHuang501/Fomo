import { fetchPortfolioSymbols, receivePortfolioSymbols} from '../actions/PortfolioSymbolActions';
import { portfolioSymbolReducer } from '../reducers/PortfolioSymbolReducer';

test("should return initial portfolio symbol state as empty when state is undefined", () => {
    const state = portfolioSymbolReducer(undefined, undefined);
    expect(state.portfoliosToSymbols).toEqual({});
});

test("should return initial portfolio symbol state as empty when state is undefined", () => {
    const initialState = { portfoliosToSymbols: {} };
    const portfolioId = 2;
    const symbols = [
        { exchange: "NYSE", symbolName: "TESLA" },
        { exchange: "NYSE", symbolName: "GOOG" }
    ];
    const fetchPortfolioSymbolsActions = receivePortfolioSymbols(portfolioId, symbols);
    const state = portfolioSymbolReducer(initialState, fetchPortfolioSymbolsActions);
    expect(state.portfoliosToSymbols.hasOwnProperty(portfolioId)).toBeTruthy();

    expect(state.portfoliosToSymbols[portfolioId][0].exchange).toEqual("NYSE");
    expect(state.portfoliosToSymbols[portfolioId][0].symbolName).toEqual("TESLA");

    expect(state.portfoliosToSymbols[portfolioId][1].exchange).toEqual("NYSE");
    expect(state.portfoliosToSymbols[portfolioId][1].symbolName).toEqual("GOOG");
});
