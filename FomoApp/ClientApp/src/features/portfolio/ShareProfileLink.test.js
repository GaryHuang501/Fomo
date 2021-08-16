import { fireEvent, screen, waitFor } from '@testing-library/react';

import React from 'react';
import ShareProfileLink from './ShareProfileLink';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';

const originalClipboard = { ...global.navigator.clipboard };

beforeEach(() => {
    const mockClipboard = {
        writeText: jest.fn(),
        readText: jest.fn()
    };
    global.navigator.clipboard = mockClipboard;
});

afterEach(() => {
    jest.resetAllMocks();
    global.navigator.clipboard = originalClipboard;
});

it("Should show tool tip when clicked", async () => {
    act(() => {
      render(<ShareProfileLink userId='123'></ShareProfileLink>);
    });
    
    expect(screen.queryByRole("alert")).not.toBeInTheDocument();
    const button = screen.getByRole("button");

    fireEvent.click(button);

    await waitFor( () => expect(screen.getByRole("alert")).toBeInTheDocument());
});

it("Should copy profile link to clipboard when clicked", async () => {
    act(() => {
      render(<ShareProfileLink userId='123'></ShareProfileLink>);
    });
    
    const button = screen.getByRole("button");
    fireEvent.click(button);

    await waitFor( () => expect(navigator.clipboard.writeText).toHaveBeenCalledWith("http://localhost/portfolio/123"));
});
