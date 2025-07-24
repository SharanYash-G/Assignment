import React, { useEffect, useState, useCallback } from "react";
import Select from "react-select";
import {
  getUsersForProcedure,
  assignUserToProcedure,
  removeUserFromProcedure,
  addProcedureToPlan,
  removeAllUsersFromProcedure,
} from "../../../api/api";

const ProcedureAssignment = ({ procedure, users, planId }) => {
  const [assignedUsers, setAssignedUsers] = useState([]);

  const identifiers = {
    planId,
    procedureId: procedure.procedureId,
  };

  // Load currently assigned users
  useEffect(() => {
    let active = true;

    const fetchAssignedUsers = async () => {
      try {
        const response = await getUsersForProcedure(identifiers.planId, identifiers.procedureId);
        const formatted = response.map(({ userId, name }) => ({
          value: userId,
          label: name,
        }));
        if (active) setAssignedUsers(formatted);
      } catch (error) {
        console.error("Failed to fetch assigned users", error);
      }
    };

    fetchAssignedUsers();

    return () => {
      active = false;
    };
  }, [identifiers.planId, identifiers.procedureId]);

  // Handle selection updates
  const handleAssignmentChange = useCallback(
    async (newSelection) => {
      const prevIds = assignedUsers.map((u) => u.value);
      const nextIds = newSelection.map((u) => u.value);

      const toAdd = nextIds.filter((id) => !prevIds.includes(id));
      const toRemove = prevIds.filter((id) => !nextIds.includes(id));

      try {
        if (nextIds.length === 0 && prevIds.length > 0) {
          await removeAllUsersFromProcedure(identifiers.planId, identifiers.procedureId);
          setAssignedUsers([]);
          return;
        }

        await addProcedureToPlan(identifiers.planId, identifiers.procedureId);

        await Promise.all([
          ...toAdd.map((id) =>
            assignUserToProcedure(identifiers.planId, identifiers.procedureId, id)
          ),
          ...toRemove.map((id) =>
            removeUserFromProcedure(identifiers.planId, identifiers.procedureId, id)
          ),
        ]);

        setAssignedUsers(newSelection);
      } catch (error) {
        console.error("Failed to update assignments", error);
      }
    },
    [assignedUsers, identifiers.planId, identifiers.procedureId]
  );

  return (
    <div className='py-2'>
      <div>{procedure.procedureTitle}</div>
      <Select
        className='mt-2'
        placeholder='Assign users'
        isMulti
        options={users}
        value={assignedUsers}
        onChange={handleAssignmentChange}
      />
    </div>
  );
};

export default ProcedureAssignment;
