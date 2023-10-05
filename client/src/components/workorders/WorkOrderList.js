//$ What does this component do?
//& this component renders the jsx of our work orders page in the browser
//& It has useffects and useStates to set the state of workOrders and mechanics (so far)
//&

import { useEffect, useState } from "react";
import { Button, Input, Table } from "reactstrap";
import { getIncompleteWorkOrders, updateWorkOrder, finishedWorkOrder, deleteThisWorkOrder } from "../../managers/workOrderManager";
import { Link } from "react-router-dom";
import { getUserProfiles } from "../../managers/userProfileManager";

//* I just pasted this here and it looks like an object initializer...we can delete it as it was test data...I collapsed that whole block of code
//# "This component has some test data in it. You can do this when you are building components that don't have endpoints yet. If you just want to work on the UI, create some fake data!"
// const testWorkOrders = [
//   {
//     id: 1,
//     description: "bent fork",
//     dateInitiated: "2023-07-12T00:00:00",
//     dateCompleted: null,
//     userProfile: null,
//     userProfileId: null,
//     bikeId: 1,
//     bike: {
//       id: 1,
//       brand: "Huffy",
//       color: "red",
//       ownerId: 1,
//       bikeTypeId: 1,
//       bikeType: {
//         id: 1,
//         name: "Mountain",
//       },
//       owner: {
//         id: 1,
//         name: "bob",
//         address: "101 main street",
//         email: "bob@bob.comx",
//         telephone: "123-456-7890",
//       },
//     },
//   },
//   {
//     id: 2,
//     description: "broken brakes",
//     dateInitiated: "2023-07-15T00:00:00",
//     dateCompleted: null,
//     userProfile: {
//       id: 1,
//       firstName: "Tony",
//       lastName: "The Tiger",
//       address: "102 fourth street",
//       email: "tony@thetiger.comx",
//       roles: ["Admin"],
//       identityUserId: "asdgfasdfvousdag",
//     },
//     userProfileId: 1,
//     bikeId: 2,
//     bike: {
//       id: 2,
//       brand: "Schwinn",
//       color: "blue",
//       ownerId: 2,
//       bikeTypeId: 1,
//       bikeType: {
//         id: 1,
//         name: "Mountain",
//       },
//       owner: {
//         id: 2,
//         name: "andy",
//         address: "101 main street",
//         email: "andy@bob.comx",
//         telephone: "123-456-7890",
//       },
//     },
//   },
// ];

export default function WorkOrderList({ loggedInUser }) {
  const [workOrders, setWorkOrders] = useState([]);
  const [mechanics, setMechanics] = useState([]);

  useEffect(() => {
    getIncompleteWorkOrders().then(setWorkOrders);
    getUserProfiles().then(setMechanics);
  }, []);

  useEffect(() => {
    // setWorkOrders(testWorkOrders);
    getIncompleteWorkOrders().then(setWorkOrders); // we updated this useEffect to set the state of this list of incomplete work orders to the most current list. Maybe more correctly said, this updates the state of the entire component
  }, []);


  //^ function to assign a mechanic to a work order
  const assignMechanic = (workOrder, mechanicId) => {
    const clone = structuredClone(workOrder);
    clone.userProfileId = mechanicId || null;
    updateWorkOrder(clone).then(() => {
      getIncompleteWorkOrders().then(setWorkOrders);
    });
  };


  //^ function to complete a work order
  const completeWorkOrder = (workOrderId) => {
    // function that expects a workOrder id number to be provided to this function....ie - a user asks to complete work order number 5...
    const updatedWorkOrders = workOrders.map((wo) => {
      // create a variable to hold the results of a map over the list of work orders and look at the id for each wo (work order)...
      if (wo.id === workOrderId) {
        // and if that wo id is the same as the workOrderId of 5 given to us by the user... then...
        return { ...wo, DateCompleted: new Date() }; //...  return the work order but with an updated value of today's date on the DateCompleted property...
      }
      return wo; //? otherwise, just return the work order....? what?
    });
    finishedWorkOrder({ id: workOrderId, DateCompleted: new Date() }).then(
      () => {
        //feeding the new values for these properties?
        setWorkOrders(updatedWorkOrders); //now set the workOrders list to the most current state (which we defined above in updatedWorkOrders variable), which is one less than before since we just completed work order number 5
      }
    );
    window.location.reload();
  };


  //^ function to delete a work order
  const deleteWorkOrder = (workOrderId) => {
    // Send an HTTP DELETE request to delete the work order
    deleteThisWorkOrder(workOrderId) // this says, run the deleteThisWorkOrder function on the selected workOrderId, which will run the DELETE method on that object in the database
      .then(() => {
        // After successfully deleting the work order on the server, update the UI
        const updatedWorkOrders = workOrders.filter(
          (wo) => wo.id !== workOrderId
        );
        setWorkOrders(updatedWorkOrders);
      })
      .catch((error) => {
        // Handle any errors that may occur during the API request
        console.error("Error deleting work order:", error);
      });
  };

  return (
    <>
      <h2>Open Work Orders</h2>
      <Link to="/workorders/create">New Work Order</Link>
      <Table>
        <thead>
          <tr>
            <th>Owner</th>
            <th>Brand</th>
            <th>Color</th>
            <th>Description</th>
            <th>DateSubmitted</th>
            <th>Mechanic</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {workOrders.map((wo) => (
            <tr key={wo.id}>
              <th scope="row">{wo.bike.owner.name}</th>
              <td>{wo.bike.brand}</td>
              <td>{wo.bike.color}</td>
              <td>{wo.description}</td>
              <td>{new Date(wo.dateInitiated).toLocaleDateString()}</td>
              <td>
                {wo.userProfile
                  ? `${wo.userProfile.firstName} ${wo.userProfile.lastName}`
                  : "unassigned"}
              </td>
              <td>
                <Input
                  type="select"
                  onChange={(e) => {
                    assignMechanic(wo, parseInt(e.target.value));
                  }}
                  value={wo.userProfileId || 0}
                >
                  <option value="0">Choose mechanic</option>
                  {mechanics.map((m) => (
                    <option
                      key={m.id}
                      value={m.id}
                    >{`${m.firstName} ${m.lastName}`}</option>
                  ))}
                </Input>
              </td>
              <td>
                {wo.userProfile && (
                  <>
                    <Button
                      onClick={() => completeWorkOrder(wo.id)}
                      color="success"
                      style={{ marginRight: '8px' }} // Add right margin for spacing
                    >
                      Mark as Complete
                    </Button>
                    <Button
                      onClick={() => deleteWorkOrder(wo.id)}
                      color="danger"
                      style={{ marginLeft: '8px' }} // Add left margin for spacing
                    >
                      Delete Work Order
                    </Button>
                    </>
                    )}
              </td>
              <td></td>
            </tr>
          ))}
        </tbody>
      </Table>
    </>
  );
}
