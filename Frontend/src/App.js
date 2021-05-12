import {
    BrowserRouter as Router,
    Switch,
    Route
} from "react-router-dom";
import { applyMiddleware, createStore } from 'redux';
import { composeWithDevTools } from 'redux-devtools-extension';
import reduxThunk from 'redux-thunk';
import { Provider } from 'react-redux';
import AppReducers from './AppReducers/AppReducers'
import LoginPage from "./Components/LoginPage";
import MainPage from "./Components/MainPage";
import ProfilePage from "./Components/ProfilePage";
import './App.css';
import ManageVoters from "./Components/ManageVoters";
import AcceptReject from "./Components/AcceptReject";

export const store = createStore(AppReducers, {}, composeWithDevTools((applyMiddleware(reduxThunk))));

function App() {
    return (
        <Provider store={store}>
            <Router>
                <Switch>
                    <Route exact path="/login">
                        <LoginPage />
                    </Route>
                    <Route exact path="/">
                        <MainPage userType />
                    </Route>
                    <Route path="/Profile" exact>
                        <ProfilePage userType />
                    </Route>
                    <Route exact path="/ManagementPanel">
                        <MainPage userType />
                    </Route>
                    <Route exact path="/VotersManagementPanel">
                        <ManageVoters userType />
                    </Route>
                    <Route exact path="/AcceptVoters">
                        <AcceptReject userType />
                    </Route>
                </Switch>
            </Router>
        </Provider>
    );
}

export default App;
