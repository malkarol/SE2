import {
  BrowserRouter as Router,
  Switch,
  Route
} from "react-router-dom";
import './App.css';
import LoginPage from "./Components/LoginPage";
import MainPage from "./Components/MainPage";
import ProfilePage from "./Components/ProfilePage";
function App() {
  return (
    <Router>
      <Switch>
        <Route path="/login" exact>
          <LoginPage />
        </Route>
        <Route path="/" exact>
          <MainPage />
        </Route>
        <Route path="/Profile" exact>
          <ProfilePage />
        </Route>
        <Route path="/ManagementPanel" exact>
          <MainPage isCoordinator/>
        </Route>
      </Switch>
    </Router>
  );
}

export default App;
