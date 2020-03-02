import * as LoginActions from "./LoginActions";

test("Should create new unauthorized action action", () => {

    var action = LoginActions.unauthorizedResponse();
    expect(action.type).toBe(LoginActions.UNAUTHORIZED_STATUS);
});

