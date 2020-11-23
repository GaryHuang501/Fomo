import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';
import config from '../../app/Config.json'

export const fetchPortfolioIds = createAsyncThunk('portfolio/fetchPortfoliIds', async (thunkApi) => {
    const response = await axios.get(`${config.apiUrl}/portfolios`);

    return response.data;
});

export const fetchPortfolio = createAsyncThunk('portfolio/fetchPortfolio', async (id, thunkApi) => {
    const response = await axios.get(`${config.apiUrl}/portfolios/${id}`);

    return response.data;
});

export const addPortfolioStock = createAsyncThunk('portfolio/addPortfolioStock', async (payload, thunkApi) => {
    const response = await axios.post(`${config.apiUrl}/portfolios/${payload.portfolioId}/portfolioSymbols`, {symbolId: payload.symbolId});

    return response.data;
});

export const portfolioSlice = createSlice({
    name: 'portfolio',
    initialState: {
        ids: [],
        selectedPortfolioId: null,
        portfolios: {

        }
    },
    reducers: {
        setSelectedPortfolioId: (state, action) => {
            state.selectedPortfolioId = action.payload;
        }
    },
    extraReducers: {
        [fetchPortfolioIds.pending]: (state, action) => {
            state.status = 'loading';
        },
        [fetchPortfolioIds.fulfilled]: (state, action) => {
            state.status = 'succeeded';
            state.ids = action.payload ?? [];
        },
        [fetchPortfolioIds.rejected]: (state, action) => {
            state.status = 'failed';
        },
        [fetchPortfolio.pending]: (state, action) => {
            state.status = 'loading';
        },
        [fetchPortfolio.fulfilled]: (state, action) => {
            state.status = 'succeeded';
            state.portfolios[action.meta.arg] = action.payload;
        },
        [fetchPortfolio.rejected]: (state, action) => {
            state.status = 'failed';
        }
    }
});

export const { setSelectedPortfolioId } = portfolioSlice.actions;

export const selectPortfolioIds = state => state.portfolio.ids;

export const selectSelectedPortfolioId = state => state.portfolio.selectedPortfolioId;

export const selectPortfolio = function(state){  
    return state.portfolio.portfolios[state.portfolio.selectedPortfolioId] ?? {symbol:[]};
}

export default portfolioSlice.reducer;
