using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using C3DSERVERLib;
namespace CSharpSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
                C3DSERVERLib.C3D m_c3d = new C3DSERVERLib.C3D();

        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenC3dFile(openFileDialog1.FileName);
            }
        }
        private void AddLog(String s)
        {
            textBox1.Text = textBox1.Text + Environment.NewLine + s;
        }
        private void OpenC3dFile(String sFilePath)
        {
            textBox1.Text = "";
            C3DSERVERLib.C3D m_c3d;
            Boolean IsOpen = false;
            m_c3d = new C3DSERVERLib.C3D();
           
            try
            {
                if (m_c3d.Open(sFilePath, 3) == 0)
                {
                    AddLog("File was opened");
                    IsOpen = true;
                    int nParamIndex = m_c3d.GetParameterIndex("TEST", "HELLO_WORLD");
                    if (nParamIndex < 0)
                    {
                        AddLog("Could not find Parameter TEST:HELLO_WORLD");
                        int nGroupIndex = m_c3d.GetGroupIndex("TEST");
                        if (nGroupIndex < 0)
                        {
                            AddLog("Could not find Group TEST");
                            nGroupIndex = m_c3d.AddGroup(0, "TEST", "Test Group by c3d server sample", 0);
                            if (nGroupIndex >= 0)
                            {
                                AddLog("Group TEST was added");

                            }
                            else
                            {
                                AddLog("Group TEST could not be added");
                            }
                        }
                        if (nGroupIndex >= 0)
                        {
                            int nGroupNumber = m_c3d.GetGroupNumber(nGroupIndex);
                            Byte[] vDimensions = { 16, 3 };
                            String[] vData = { "Data Item 1", "Data Item 2", "Data Item 3" };
                            nParamIndex = m_c3d.AddParameter("HELLO_WORLD", "Hello World Test", "TEST", 0, -1, 2, vDimensions, vData);
                            if (nParamIndex >= 0)
                            {
                                AddLog("Parameter TEST:HELLO_WORLD was created");
                            }
                            else
                            {
                                AddLog("Parameter could not be created");

                            }
                        }

                    }
                    else
                    {
                        int nDone;
                        nDone = m_c3d.DeleteParameter(nParamIndex);
                        if (nDone == 1)
                        {
                            AddLog("Parameter TEST:HELLO_WORLD was deleted");
                            int nGroupIndex  = m_c3d.GetGroupIndex("TEST");
                            if (nGroupIndex >= 0)
                            {
                                    nDone = m_c3d.DeleteGroup(nGroupIndex, 0);
                                    if (nDone == 1)
                                    {
                                        AddLog("Group TEST has been deleted");
                                    }
                                    else
                                    {
                                        AddLog("Group TEST could not be deleted");

                                    }
                            }
                        }
                        else
                        {
                            AddLog("Parameter could not be deleted");

                        }
                    }

                    int nSaved = m_c3d.SaveFile(sFilePath, m_c3d.GetFileType());
                    if (nSaved == 1)
                    {
                        AddLog("File was saved");
                    }
                    else
                    {
                        AddLog("File could not be saved");
                    }
                    m_c3d.Close();
                }
                else
                {
                    MessageBox.Show("Could not open the file");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (IsOpen)
                {
                    m_c3d.Close();
                }
            }
        }
    }
}
