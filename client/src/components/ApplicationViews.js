import { Route, Routes } from "react-router-dom";
import Bikes from "./bikes/Bikes";
import { AuthorizedRoute } from "./auth/AuthorizedRoute";
import Login from "./auth/Login";
import Register from "./auth/Register";
import WorkOrderList from "./workorders/WorkOrderList";
import CreateWorkOrder from "./workorders/CreateWorkOrder";

export default function ApplicationViews({ loggedInUser, setLoggedInUser }) {
  return (
    <Routes>
      <Route path="/">
        <Route
          index
          element={
            <AuthorizedRoute loggedInUser={loggedInUser}>
              <Bikes />
            </AuthorizedRoute>
          }
        />
        <Route
          path="bikes"
          element={
            <AuthorizedRoute loggedInUser={loggedInUser}>
              <Bikes />
            </AuthorizedRoute>
          }
        />
        {/* Below: 
        "The Route group create two routes for workorders. 
        The route marked index will match to workorders with no extra url segments. 
        The create route will match /workorders/create." */}
        <Route path="workorders"> 
          <Route
            index
            element={
              <AuthorizedRoute loggedInUser={loggedInUser}>
                <WorkOrderList />
              </AuthorizedRoute>
            }
          />
          <Route
            path="create"
            element={
              <AuthorizedRoute loggedInUser={loggedInUser}>
                <CreateWorkOrder />
              </AuthorizedRoute>
            }
          />
        </Route>
        <Route
          path="employees"
          element={
            <AuthorizedRoute roles={["Admin"]} loggedInUser={loggedInUser}>
              <p>Employees</p>
            </AuthorizedRoute>
          }
        />
        <Route
          path="login"
          element={<Login setLoggedInUser={setLoggedInUser} />}
        />
        <Route
          path="register"
          element={<Register setLoggedInUser={setLoggedInUser} />}
        />
      </Route>
      <Route path="*" element={<p>Whoops, nothing here...</p>} />
    </Routes>
  );
}
