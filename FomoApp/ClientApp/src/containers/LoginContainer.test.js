import { mapStateToProps } from './LoginContainer';

test("Should map default isUnauthorized correctly to props", () => {
    var state = {
        loginReducer: { isUnauthorized: true }
    };
    var mappedState = mapStateToProps(state);
    expect(mappedState).toEqual({ isUnauthorized: true });
});

