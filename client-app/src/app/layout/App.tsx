import React, { Fragment, useState, useEffect } from "react"
import axios from "axios"
import { Container } from "semantic-ui-react"
import { IActivity } from "../models/activity"
import NavBar from "../../features/nav/NavBar"
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard'


const App: React.FC = () => {

  const [activities, setActivities] = useState<IActivity[]>([])
  const [selectedActivity, setSelectedActivity] = useState<IActivity | null>(null)
  const [editMode, setEditMode] = useState(false)

  const handleSelectActivity = (id: string) => { setSelectedActivity(activities.filter(a => a.id === id)[0]); setEditMode(false) }
  const handleOpenCreateForm = () => { setSelectedActivity(null); setEditMode(true) }
  const handleCreateActivity = (activity: IActivity) => { setActivities([...activities, activity]); setSelectedActivity(activity); setEditMode(false) }
  const handleDeleteActivity = (id: string) => { setActivities([...activities.filter(a => a.id !== id)]) }
  const handleEditActivity = (activity: IActivity) => {
    setActivities([...activities.filter(a => a.id !== activity.id), activity]); setSelectedActivity(activity); setEditMode(false)
  }
  const formatDate = (activities: IActivity[]): IActivity[] => {
    let newActivities: IActivity[] = []
    activities.forEach(activity => { activity.date = activity.date.split('.')[0]; newActivities.push(activity) })
    return newActivities
  }

  useEffect(() => {
    axios
      .get<IActivity[]>("http://localhost:5000/api/activities")
      .then(response => { setActivities(formatDate(response.data)) })
  }, [])

  return (
    <Fragment>
      <NavBar openCreateForm={handleOpenCreateForm} />
      <Container style={{ marginTop: '7em' }}>
        <ActivityDashboard
          activities={activities} selectActivity={handleSelectActivity} selectedActivity={selectedActivity} editMode={editMode} setEditMode={setEditMode}
          setSelectedActivity={setSelectedActivity} createActivity={handleCreateActivity} editActivity={handleEditActivity}
          deleteActivity={handleDeleteActivity}
        />
      </Container>
    </Fragment>
  )
}



export default App

// notes: 
// useEffect doesn't go well with async/await, 
// will only work when async/await is wrapped in another (fetchData) function.
// this noise I want to avoid more than killing '.then'.
