import { fireEvent, screen, waitFor } from '@testing-library/react';

import ProfileSettings from './ProfileSettings';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

beforeEach(() => {
    process.env = {
        REACT_APP_MIN_NAME_LENGTH: 3,
        REACT_APP_MAX_NAME_LENGTH: 50
    };
});

it("Should submit updated user", async () => {

    const myUser = { name: "myUser"};
    let updatedUser;

    act(() => {
        render(<ProfileSettings myUser={myUser} onSubmit={(u) => { updatedUser = u; }} />);
    });

    const inputs = screen.getAllByRole('textbox');
    const submit = screen.getByRole('button');

    const nameInput = inputs[0];

    fireEvent.change(nameInput, { target: { value: 'updatedName' } });
    fireEvent.click(submit);

    await waitFor(() => {
        expect(updatedUser.name).toBe("updatedName");
    });
});

it("Should show heading", async () => {

    act(() => {
        render(<ProfileSettings heading={"hello"} onSubmit={(u) => { }} />);
    });

    expect(screen.getByText("hello")).toBeInTheDocument();
});


it("Should show error that new name is too short", async () => {

    const myUser = { name: "myUser"};
    let updatedUser;

    act(() => {
        render(<ProfileSettings myUser={myUser} onSubmit={(u) => { updatedUser = u; }} />);
    });

    const inputs = screen.getAllByRole('textbox');
    const submit = screen.getByRole('button');

    const nameInput = inputs[0];

    fireEvent.change(nameInput, { target: { value: '12' } });
    fireEvent.click(submit);

    await waitFor(() => {
        expect(screen.getByText(`Username must be within ${process.env.REACT_APP_MIN_NAME_LENGTH} and ${process.env.REACT_APP_MAX_NAME_LENGTH} characters.`)).toBeInTheDocument();
    });
});


it("Should show error that new name is too long", async () => {

    process.env.REACT_APP_MAX_NAME_LENGTH = 5;
    const myUser = { name: "myUser"};
    let updatedUser;

    act(() => {
        render(<ProfileSettings myUser={myUser} onSubmit={(u) => { updatedUser = u; }} />);
    });

    const inputs = screen.getAllByRole('textbox');
    const submit = screen.getByRole('button');

    const nameInput = inputs[0];

    fireEvent.change(nameInput, { target: { value: 'updatedName' } });
    fireEvent.click(submit);

    await waitFor(() => {
        expect(screen.getByText(`Username must be within ${process.env.REACT_APP_MIN_NAME_LENGTH} and ${process.env.REACT_APP_MAX_NAME_LENGTH} characters.`)).toBeInTheDocument();
    });
});

it("Should any api errors", async () => {

    const myUser = { name: "myUser"};

    act(() => {
        render(<ProfileSettings myUser={myUser} apiErrors={"Error"} />);
    });

    await waitFor(() => {
        expect(screen.getByText('Error')).toBeInTheDocument();
    });
});
