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

export const addPortfolioStock = createAsyncThunk('portfolio/addPortfolioStock', async (thunkApi) => {

});

export const portfolioSlice = createSlice({
    name: 'portfolio',
    initialState: {
        ids: []
    },
    reducers: {
    },
    extraReducers: {
        [fetchPortfolioIds.pending]: (state, action) => {
            state.status = 'loading';
        },
        [fetchPortfolioIds.fulfilled]: (state, action) => {
            state.status = 'succeeded';
            state.portfolio.ids = action.payload ?? [];
        },
        [fetchPortfolioIds.rejected]: (state, action) => {
            state.status = 'failed';
        },
        [fetchPortfolio.pending]: (state, action) => {
            state.status = 'loading';
        },
        [fetchPortfolio.fulfilled]: (state, action) => {
            state.status = 'succeeded';
            state.portfolio[action.meta.arg] = action.payload;
        },
        [fetchPortfolio.rejected]: (state, action) => {
            state.status = 'failed';
        }
    }
});

export const selectPortfolioIds = state => state.portfolio.ids;


export default portfolioSlice.reducer;
