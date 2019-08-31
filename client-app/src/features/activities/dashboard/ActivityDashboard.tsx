import React, { useContext, useEffect } from 'react'
import { Grid } from 'semantic-ui-react'
import { observer } from 'mobx-react-lite'
import ActivityList from './ActivityList'
import LoaderComponent from '../../../app/layout/LoaderComponent'
import { RootStoreContext } from '../../../app/stores/rootStore';


const ActivityDashboard: React.FC = () => {

  const rootStore = useContext(RootStoreContext)
  const { loadActivities, loadingInitial } = rootStore.activityStore

  useEffect(() => { loadActivities() }, [loadActivities])
  if (loadingInitial) return <LoaderComponent content='Loading activities...' />
  
  return (
    <Grid>
      <Grid.Column width={10}>
        <ActivityList />
      </Grid.Column>
      <Grid.Column width={6}>
       <h2>Activity filters</h2>
      </Grid.Column>
    </Grid>
  )
}


export default observer(ActivityDashboard)