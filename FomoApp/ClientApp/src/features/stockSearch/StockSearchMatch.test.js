import { fireEvent, screen, waitFor } from '@testing-library/react';

import React from 'react';
import { StockSearchMatch } from './StockSearchMatch';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';

beforeEach(() => {
});

afterEach(() => {
});

it("renders symbol match", async () => {

    let clickCalled = false;

    const match = {
        symbolId: 1,
        ticker: 'BAC',
        fullName: 'Bank Of America',

    };

    const onClick = (arg) => {
        clickCalled = arg === match.symbolId
    };

    act(() => {
        render(<StockSearchMatch match={match} onClick={onClick}></StockSearchMatch>);
    });

    const option = screen.getByRole('option');
    expect(screen.getByText(match.ticker)).toBeInTheDocument();
    expect(screen.getByText(match.fullName)).toBeInTheDocument();

    fireEvent.click(option);
    await waitFor(() => expect(clickCalled).toBeTruthy());
});

