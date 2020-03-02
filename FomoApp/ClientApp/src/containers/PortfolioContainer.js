import { connect } from 'react-redux';
import { Portfolio } from '../components/Portfolio';
import { fetchPortfolios, switchPortfolio } from '../actions/PortfolioActions';
import { addPortfolioSymbol, fetchPortfolioSymbols } from '../actions/PortfolioSymbolActions';

const exchanges = [{ exchangeName: "NYSE", exchangeId: 1 }, { exchangeName: "NDAQ", exchangeId: 2 }];

export function mapStateToProps(state) {
    return {
        portfolios: state.portfolioReducer.portfolios,
        portfoliosToSymbols: state.portfolioSymbolReducer.portfoliosToSymbols,
        exchanges: exchanges.slice(),
        selectedPortfolioId: "123"
    };
}

export function mapDispatchToProps(dispatch) {
    return {
        onLoadPortfolioList: () => dispatch(fetchPortfolios()),
        onLoadSymbolForPortfolio: (portfolioId) => dispatch(fetchPortfolioSymbols(portfolioId)),

        onAddSymbolToPortfolio: (portfolioId, exchangeName, symbolName) =>
            dispatch(addPortfolioSymbol(portfolioId, exchangeName, symbolName)),

        onSwitchPortfolio: (portfolioId) => {
            dispatch(switchPortfolio(portfolioId));
            dispatch(fetchPortfolioSymbols(portfolioId));
        }
    };
}

export const PortfolioContainer = connect(
    mapStateToProps,
    mapDispatchToProps,
)(Portfolio);

