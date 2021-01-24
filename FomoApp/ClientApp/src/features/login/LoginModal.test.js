import { fireEvent, screen, waitFor, within } from '@testing-library/react';

import { LoginModal } from './LoginModal';
import React from 'react';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';

beforeEach(() => {
    process.env = {
        REACT_APP_API_URL: "https://test.com",
    };

    const location = new URL(process.env.REACT_APP_API_URL);
    delete window.location;
    window.location = location;
});

afterEach(() => {
    process.env = {};
});

it("Should go to google login page when click sign in for google", async () => {

    act(() => {
        render(<LoginModal/>);
    });

    fireEvent.click(screen.getByAltText("Google Sign in"));
    await waitFor(() => expect(window.location.href).toEqual(`${process.env.REACT_APP_API_URL}/accounts/login?provider=Google&returnurl=https://test.com/`));
});
