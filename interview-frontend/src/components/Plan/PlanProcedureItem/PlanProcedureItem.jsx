import React, { useEffect, useState } from "react";
import ReactSelect from "react-select";
import {
  getUsersForProcedure,
  assignUserToProcedure,
  removeUserFromProcedure,
  addProcedureToPlan,
  removeAllUsersFromProcedure,
} from "../../../api/api";

const PlanProcedureItem = ({ procedure, users, planId }) => {
  const [selectedUsers, setSelectedUsers] = useState([]);

  useEffect(() => {
    let isMounted = true; // flag to track component mount status

    (async () => {
      try {
        const assigned = await getUsersForProcedure(
          planId,
          procedure.procedureId
        );
        const mapped = assigned.map((u) => ({
          label: u.name,
          value: u.userId,
        }));

        // Only set state if component is still mounted
        if (isMounted) {
          setSelectedUsers(mapped);
        }
      } catch (err) {
        console.error("Failed to load assigned users:", err);
      }
    })();

    return () => {
      isMounted = false; // cleanup on unmount
    };
  }, [planId, procedure.procedureId]);

  const handleAssignUserToProcedure = async (selectedOptions) => {
    const previous = selectedUsers.map((u) => u.value);
    const current = selectedOptions.map((u) => u.value);

    if (selectedOptions.length === 0 && previous.length > 0) {
      try {
        await removeAllUsersFromProcedure(planId, procedure.procedureId);
        setSelectedUsers([]);
      } catch (err) {
        console.error("Error removing all assigned users:", err);
      }
      return;
    }

    const removed = previous.filter((id) => !current.includes(id));
    const added = current.filter((id) => !previous.includes(id));

    try {
      await addProcedureToPlan(planId, procedure.procedureId);

      for (const userId of added) {
        await assignUserToProcedure(planId, procedure.procedureId, userId);
      }

      for (const userId of removed) {
        await removeUserFromProcedure(planId, procedure.procedureId, userId);
      }

      setSelectedUsers(selectedOptions);
    } catch (err) {
      console.error("Error updating user assignments:", err);
    }
  };

  return (
    <div className='py-2'>
      <div>{procedure.procedureTitle}</div>

      <ReactSelect
        className='mt-2'
        placeholder='Select User to Assign'
        isMulti={true}
        options={users}
        value={selectedUsers}
        onChange={handleAssignUserToProcedure}
      />
    </div>
  );
};

export default PlanProcedureItem;
